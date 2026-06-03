using AMR.Core.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace AMR.Core.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.Sucesso) return controller.Ok(result.Valor);

        var detail = result.Erro ?? "Erro desconhecido.";
        var isNotFound = detail.Contains("não encontrado", StringComparison.OrdinalIgnoreCase);

        return isNotFound
            ? controller.Problem(detail: detail, statusCode: StatusCodes.Status404NotFound,   title: "Não encontrado")
            : controller.Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest, title: "Requisição inválida");
    }
}
