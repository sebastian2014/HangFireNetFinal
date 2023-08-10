using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Hangfire;
using System.Diagnostics;
using AccesoDatos;

namespace HangFireNet6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly IRepositorioPersonas repositorioPersonas;
        private readonly IBackgroundJobClient backgroundJobClient;

        public PersonaController(IRepositorioPersonas repositorioPersonas, IBackgroundJobClient backgroundJobClient)
        {
            this.repositorioPersonas = repositorioPersonas;
            this.backgroundJobClient = backgroundJobClient;
        }


        [HttpPost("crear")]
        public async Task<ActionResult> Crear(string nombrePersona)
        {
            backgroundJobClient.Enqueue<IRepositorioPersonas>(repo => repo.Crear(nombrePersona));

            //backgroundJobClient.Enqueue(() => Console.WriteLine(nombrePersona));
            //Console.WriteLine($"agregando a la persona {nombrePersona}");
            //var persona = new Persona { Nombre = nombrePersona, Estado = Estado.Desaprobado, Fecha = DateTime.Now };
            //await repositorioPersonas.Crear(persona);
            await Task.Delay(2000);
            Console.WriteLine($"agregada la persona {nombrePersona}");
            return Ok();
        }

        [HttpPost("schedule")]
        public ActionResult Schedule(string nombrePersona)
        {
            var jobId = backgroundJobClient.Schedule(() => Console.WriteLine("El nombre de la persona job schedule es " + nombrePersona), TimeSpan.FromSeconds(5));
            backgroundJobClient.ContinueJobWith(jobId, () => Console.WriteLine($"El job {jobId} ha concluido"));
            return Ok();
        }
        
        [HttpPost("enqueue")]
        public IActionResult EnqueueJobs()
        {
            // Escaneo de archivos en la carpeta
            string folderPath = @"C:\Users\Usuario\source\repos\HangFire-net6\HangFireNet6\hangfireFolder";
            string[] consoleFiles = Directory.GetFiles(folderPath, "*.exe");

            // Configurar trabajos para cada archivo de consola
            foreach (string file in consoleFiles)
            {
                BackgroundJob.Enqueue(() => EjecutarArchivoConsola(file));
            }

            return Ok("Trabajos encolados.");
        }

        public static void EjecutarArchivoConsola(string filePath)
        {
           // Process.Start(filePath);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                // Puedes manejar la salida del proceso según tus necesidades
            }
        }
    }
}
