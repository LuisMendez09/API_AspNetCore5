using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Utilidades
{

    public class AlmacenadorAzureStorage : IAlmacenadorArchivos
    {
        private string connectionString;

        public AlmacenadorAzureStorage(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<String> GuardarArchivo(String contenedor, IFormFile archivo)
        {
            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();

            cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var extencion = Path.GetExtension(archivo.FileName);
            var archivoNomre = $"{Guid.NewGuid()}{extencion}";

            var blod = cliente.GetBlobClient(archivoNomre);
            await blod.UploadAsync(archivo.OpenReadStream());

            return blod.Uri.ToString();
        }

        public async Task borrarArhivo(String ruta, String contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
                return;

            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            var archivo = Path.GetFileName(ruta);
            var blod = cliente.GetBlobClient(archivo);
            await blod.DeleteIfExistsAsync();
        }

        public async Task<string> editarArhivo(String contenedor, IFormFile archivo, String ruta)
        {
            await borrarArhivo(ruta, contenedor);
            return await GuardarArchivo(contenedor, archivo);
        }
    }
}
