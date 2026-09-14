using BDAplication.Web.Models;

namespace BDAplication.Web.Helpers;

/// <summary>
/// Fuente única de color para Priority / TaskBoardStatus, usada por TaskCard, TaskColumn,
/// TaskPlanner, CreateTaskDialog y EditTaskDialog. Antes cada uno duplicaba los mismos hex.
/// Devuelve nombres de variables CSS (--mud-palette-* o --ds-*), no hex fijos, para que
/// el color siga reaccionando al tema claro/oscuro.
/// </summary>
public static class PriorityPalette
{
    public static string Css(Priority priority) => priority switch
    {
        Priority.Low      => "var(--mud-palette-success)",
        Priority.Medium   => "var(--mud-palette-warning)",
        Priority.High     => "var(--mud-palette-error)",
        Priority.Critical => "var(--ds-critical)",
        _                 => "var(--mud-palette-gray-default)"
    };

    public static string CssLight(Priority priority) => priority switch
    {
        Priority.Low      => "var(--ds-success-lt)",
        Priority.Medium   => "var(--ds-warn-lt)",
        Priority.High     => "var(--ds-error-lt)",
        Priority.Critical => "var(--ds-critical-lt)",
        _                 => "var(--ds-border)"
    };

    public static string StatusCss(TaskBoardStatus status) => status switch
    {
        TaskBoardStatus.Pending    => "var(--mud-palette-primary)",
        TaskBoardStatus.InProgress => "var(--mud-palette-warning)",
        TaskBoardStatus.Suspended  => "var(--ds-suspended)",
        TaskBoardStatus.Cancelled  => "var(--mud-palette-error)",
        TaskBoardStatus.Completed  => "var(--mud-palette-success)",
        _                          => "var(--mud-palette-gray-default)"
    };
}
