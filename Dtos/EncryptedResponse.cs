
namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos;

public class EncryptedResponse
{
    public string EncryptedData { get; set; }

    public EncryptedResponse(string encryptedData)
    {
        EncryptedData = encryptedData;
    }

}
