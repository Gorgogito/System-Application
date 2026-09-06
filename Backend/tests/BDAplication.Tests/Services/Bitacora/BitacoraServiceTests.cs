using BitacoraEntity = BDAplication.Domain.Entities.Bitacora.Bitacora;
using BDAplication.Application.DTOs.Bitacora;
using BDAplication.Application.Interfaces;
using BDAplication.Application.Services.Bitacora;
using BDAplication.Domain.Entities.Bitacora;
using BDAplication.Domain.Enums;
using BDAplication.Domain.Interfaces.Bitacora;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BDAplication.Tests.Services.Bitacora;

public class BitacoraServiceTests
{
    private readonly Mock<IBitacoraRepository> _repoMock = new();
    private readonly Mock<IBlobStorageService> _blobMock = new();
    private readonly BitacoraService _service;

    public BitacoraServiceTests()
    {
        IConfiguration config = new ConfigurationBuilder().Build(); // usa los defaults del servicio
        _service = new BitacoraService(_repoMock.Object, _blobMock.Object, config);
    }

    private static BitacoraEntity Dia(int id = 1, int userId = 10) => new()
    {
        Id = id,
        UserId = userId,
        Fecha = new DateTime(2026, 1, 1),
        UserCreated = "user1"
    };

    private static BitacoraActividad Actividad(int id, BitacoraEntity bitacora, TimeOnly inicio, TimeOnly fin) => new()
    {
        Id = id,
        BitacoraId = bitacora.Id,
        Bitacora = bitacora,
        HoraInicio = inicio,
        HoraFin = fin,
        Descripcion = "Actividad",
        UserCreated = "user1",
        IsActive = true
    };

    // ── Crear actividad ──────────────────────────────────────

    [Fact]
    public async Task CreateActividadAsync_ValidRequest_CreatesActividad()
    {
        var bitacora = Dia();
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bitacora);
        _repoMock.Setup(r => r.GetActividadesActivasByBitacoraIdAsync(1, null)).ReturnsAsync(new List<BitacoraActividad>());
        _repoMock.Setup(r => r.CreateActividadAsync(It.IsAny<BitacoraActividad>()))
            .ReturnsAsync((BitacoraActividad a) => { a.Id = 100; return a; });

        var request = new CreateActividadRequest(1, new TimeOnly(9, 0), new TimeOnly(9, 30), "Desayuné");
        var result = await _service.CreateActividadAsync(request, userId: 10, user: "user1");

        result.Id.Should().Be(100);
        result.Descripcion.Should().Be("Desayuné");
    }

    [Fact]
    public async Task CreateActividadAsync_Overlapping_ThrowsArgumentException()
    {
        var bitacora = Dia();
        var existente = Actividad(1, bitacora, new TimeOnly(9, 0), new TimeOnly(11, 0));
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bitacora);
        _repoMock.Setup(r => r.GetActividadesActivasByBitacoraIdAsync(1, null)).ReturnsAsync(new List<BitacoraActividad> { existente });

        var request = new CreateActividadRequest(1, new TimeOnly(10, 30), new TimeOnly(12, 0), "Otra actividad");

        var act = () => _service.CreateActividadAsync(request, userId: 10, user: "user1");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateActividadAsync_ContiguousBoundary_DoesNotThrow()
    {
        var bitacora = Dia();
        var existente = Actividad(1, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bitacora);
        _repoMock.Setup(r => r.GetActividadesActivasByBitacoraIdAsync(1, null)).ReturnsAsync(new List<BitacoraActividad> { existente });
        _repoMock.Setup(r => r.CreateActividadAsync(It.IsAny<BitacoraActividad>()))
            .ReturnsAsync((BitacoraActividad a) => { a.Id = 101; return a; });

        // Empieza justo cuando termina la anterior — no es solape
        var request = new CreateActividadRequest(1, new TimeOnly(9, 30), new TimeOnly(13, 0), "Hice ejercicios");

        var result = await _service.CreateActividadAsync(request, userId: 10, user: "user1");

        result.Id.Should().Be(101);
    }

    [Fact]
    public async Task CreateActividadAsync_HoraFinAntesDeHoraInicio_ThrowsArgumentException()
    {
        var bitacora = Dia();
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bitacora);

        var request = new CreateActividadRequest(1, new TimeOnly(14, 0), new TimeOnly(13, 0), "Actividad inválida");

        var act = () => _service.CreateActividadAsync(request, userId: 10, user: "user1");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateActividadAsync_BitacoraDeOtroUsuario_ThrowsUnauthorizedAccessException()
    {
        var bitacora = Dia(userId: 10);
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bitacora);

        var request = new CreateActividadRequest(1, new TimeOnly(9, 0), new TimeOnly(9, 30), "Desayuné");

        var act = () => _service.CreateActividadAsync(request, userId: 999, user: "otro");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    // ── Eliminar actividad ───────────────────────────────────

    [Fact]
    public async Task DeleteActividadAsync_ConEvidencias_EliminaBlobsYSoftDeleteActividad()
    {
        var bitacora = Dia();
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        actividad.Evidencias.Add(new BitacoraEvidencia { Id = 1, BitacoraActividadId = 5, BlobPath = "bitacora/10/1/5/a.jpg" });
        _repoMock.Setup(r => r.GetActividadByIdAsync(5)).ReturnsAsync(actividad);

        await _service.DeleteActividadAsync(5, userId: 10);

        _blobMock.Verify(b => b.DeleteAsync("bitacora/10/1/5/a.jpg"), Times.Once);
        _repoMock.Verify(r => r.DeleteEvidenciaAsync(It.IsAny<BitacoraEvidencia>()), Times.Once);
        _repoMock.Verify(r => r.DeleteActividadAsync(actividad), Times.Once);
    }

    // ── Evidencia — subida directa (imagen) ──────────────────

    private static byte[] JpegBytes(int length = 64)
    {
        var bytes = new byte[length];
        bytes[0] = 0xFF; bytes[1] = 0xD8; bytes[2] = 0xFF;
        return bytes;
    }

    [Fact]
    public async Task UploadEvidenciaAsync_ExtensionNoPermitida_ThrowsArgumentException()
    {
        var bitacora = Dia();
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        _repoMock.Setup(r => r.GetActividadByIdAsync(5)).ReturnsAsync(actividad);

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var act = () => _service.UploadEvidenciaAsync(5, stream, "malware.exe", "application/octet-stream", 3, 10, "user1");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UploadEvidenciaAsync_ExtensionDeVideo_ThrowsArgumentException()
    {
        var bitacora = Dia();
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        _repoMock.Setup(r => r.GetActividadByIdAsync(5)).ReturnsAsync(actividad);

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var act = () => _service.UploadEvidenciaAsync(5, stream, "video.mp4", "video/mp4", 3, 10, "user1");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*carga directa*");
    }

    [Fact]
    public async Task UploadEvidenciaAsync_FirmaBinariaNoCoincide_ThrowsArgumentException()
    {
        var bitacora = Dia();
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        _repoMock.Setup(r => r.GetActividadByIdAsync(5)).ReturnsAsync(actividad);
        _repoMock.Setup(r => r.CountEvidenciasByActividadAsync(5)).ReturnsAsync(0);

        // Extensión .jpg pero contenido que no corresponde a la firma JPEG real
        var falso = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        using var stream = new MemoryStream(falso);

        var act = () => _service.UploadEvidenciaAsync(5, stream, "foto.jpg", "image/jpeg", falso.Length, 10, "user1");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*firma binaria*");
    }

    [Fact]
    public async Task UploadEvidenciaAsync_ArchivoValido_CreaEvidencia()
    {
        var bitacora = Dia();
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        _repoMock.Setup(r => r.GetActividadByIdAsync(5)).ReturnsAsync(actividad);
        _repoMock.Setup(r => r.CountEvidenciasByActividadAsync(5)).ReturnsAsync(0);
        _repoMock.Setup(r => r.CreateEvidenciaAsync(It.IsAny<BitacoraEvidencia>()))
            .ReturnsAsync((BitacoraEvidencia e) => { e.Id = 50; return e; });

        var jpeg = JpegBytes();
        using var stream = new MemoryStream(jpeg);

        var result = await _service.UploadEvidenciaAsync(5, stream, "foto.jpg", "image/jpeg", jpeg.Length, 10, "user1");

        result.Id.Should().Be(50);
        result.Tipo.Should().Be(TipoEvidencia.Imagen.ToString());
        _blobMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), It.Is<string>(p => p.StartsWith("bitacora/10/1/5/")), "image/jpeg"), Times.Once);
    }

    [Fact]
    public async Task UploadEvidenciaAsync_CuotaAlcanzada_ThrowsArgumentException()
    {
        var bitacora = Dia();
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        _repoMock.Setup(r => r.GetActividadByIdAsync(5)).ReturnsAsync(actividad);
        _repoMock.Setup(r => r.CountEvidenciasByActividadAsync(5)).ReturnsAsync(20); // límite por defecto

        var jpeg = JpegBytes();
        using var stream = new MemoryStream(jpeg);

        var act = () => _service.UploadEvidenciaAsync(5, stream, "foto.jpg", "image/jpeg", jpeg.Length, 10, "user1");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ── Evidencia — acceso y confirmación ─────────────────────

    [Fact]
    public async Task GetEvidenciaUrlAsync_DeOtroUsuario_ThrowsUnauthorizedAccessException()
    {
        var bitacora = Dia(userId: 10);
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        var evidencia = new BitacoraEvidencia { Id = 1, BitacoraActividadId = 5, Actividad = actividad, BlobPath = "x" };
        _repoMock.Setup(r => r.GetEvidenciaByIdAsync(1)).ReturnsAsync(evidencia);

        var act = () => _service.GetEvidenciaUrlAsync(1, userId: 999);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ConfirmEvidenciaAsync_BlobPathFueraDeLaActividad_ThrowsUnauthorizedAccessException()
    {
        var bitacora = Dia(userId: 10);
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        _repoMock.Setup(r => r.GetActividadByIdAsync(5)).ReturnsAsync(actividad);

        var request = new ConfirmEvidenciaRequest(5, "bitacora/10/1/999/otro.mp4", "video.mp4", 1000);

        var act = () => _service.ConfirmEvidenciaAsync(request, userId: 10, user: "user1");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    // ── Resumen ───────────────────────────────────────────────

    [Fact]
    public async Task GetResumenAsync_CuentaImagenesYVideosCorrectamente()
    {
        var bitacora = Dia();
        var actividad = Actividad(5, bitacora, new TimeOnly(9, 0), new TimeOnly(9, 30));
        actividad.Evidencias.Add(new BitacoraEvidencia { Tipo = TipoEvidencia.Imagen });
        actividad.Evidencias.Add(new BitacoraEvidencia { Tipo = TipoEvidencia.Imagen });
        actividad.Evidencias.Add(new BitacoraEvidencia { Tipo = TipoEvidencia.Video });
        bitacora.Actividades.Add(actividad);

        _repoMock.Setup(r => r.GetRangeAsync(10, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
            .ReturnsAsync(new List<BitacoraEntity> { bitacora });

        var request = new GetResumenRequest(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31), null);
        var result = (await _service.GetResumenAsync(request, userId: 10)).ToList();

        result.Should().HaveCount(1);
        result[0].TotalActividades.Should().Be(1);
        result[0].TotalImagenes.Should().Be(2);
        result[0].TotalVideos.Should().Be(1);
    }
}
