using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Services;
using Newtonsoft.Json;

namespace PruebaTecnica_DVP_Net_Kubernetes.Filters
{
    public class EncryptResponseFilter : IActionFilter
    {
        private readonly EncryptionService _encryptionService;

        public EncryptResponseFilter(EncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var originalValue = objectResult.Value;
                var resultType = originalValue.GetType();

                // Verificamos si el tipo es una respuesta genérica (GenericResponse<>)
                if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(GenericResponse<>))
                {
                    // Obtenemos las propiedades 'IsError', 'Message' y 'Data' del objeto original
                    var isErrorProperty = resultType.GetProperty("IsError");
                    var messageProperty = resultType.GetProperty("Message");
                    var dataProperty = resultType.GetProperty("Data");
                    var totalCountProperty = resultType.GetProperty("TotalCount");

                    if (isErrorProperty != null && messageProperty != null && dataProperty != null)
                    {
                        var isErrorValue = isErrorProperty.GetValue(originalValue);
                        var messageValue = messageProperty.GetValue(originalValue);
                        var dataValue = dataProperty.GetValue(originalValue);
                        int totalCountValue = int.Parse(totalCountProperty!.GetValue(originalValue)!.ToString()!);

                        if (dataValue != null)
                        {
                            // Si la encriptación está habilitada, generamos un JWT, si no, simplemente los usamos sin modificarlos
                            if (_encryptionService.IsEncryptionEnabled())
                            {
                                // Generamos el JWT
                                string jwtToken = _encryptionService.Encrypt(dataValue);

                                // Creamos un nuevo GenericResponse<string> con el token JWT
                                var newResponse = new GenericResponse<string>
                                {
                                    IsError = isErrorValue != null ? (bool)isErrorValue : false,
                                    Message = messageValue != null ? (string)messageValue : string.Empty,
                                    Data = jwtToken,
                                    TotalCount = totalCountValue
                                };

                                // Reemplazamos el valor del resultado con el nuevo objeto
                                objectResult.Value = newResponse;
                            }
                            else
                            {
                                // Si la encriptación está deshabilitada, devolvemos los datos originales sin modificarlos
                                var newResponse = new GenericResponse<object>
                                {
                                    IsError = isErrorValue != null ? (bool)isErrorValue : false,
                                    Message = messageValue != null ? (string)messageValue : string.Empty,
                                    Data = dataValue,
                                    TotalCount = totalCountValue
                                };

                                // Reemplazamos el valor del resultado con los datos sin encriptar
                                objectResult.Value = newResponse;
                            }
                        }
                    }
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context) { }
    }
}
