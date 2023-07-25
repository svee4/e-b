
using System.Net;
using e_b.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace e_b.Domain.Models.Api;

public static class Helpers
{

    /// <summary>
    /// Class for creating a response that resembles the model validation error response
    /// </summary>
    public class ModelStateResponse
    {
        public Dictionary<string, string[]> Errors { get; set; }
        public int Status { get; set; }

        public static ModelStateResponse FromModelState(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState, int status = 400)
        {
            return new()
            {
                Errors = modelState
                            .Where(entry => entry.Value is not null && entry.Value.Errors.Count > 0)
                            .ToDictionary(entry => entry.Key, entry => entry.Value!.Errors.Select(err => err.ErrorMessage).ToArray()),
                Status = status
            };
        }
    }


}