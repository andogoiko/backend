using System;
using BDD;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProcesoBackend
{

    class BalizLoc
    {
        public string Baliza { get; set; }
        public string Localidad { get; set; }
    }
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Comenzarem Prugrama");

            /* cuenta atras de 10 minutos */

            TimeSpan countdown = new TimeSpan(0, 0, 600);

            /* Bucle para hacer la expropiación de datos infinitamente*/

            while (true)
            {
                /* añadimos un cliente para hacer la request a la API */

                var _cliente = new HttpClient();

                /* JWT */

                _cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                /* APIs */

                var APILocalidades = "https://www.euskalmet.euskadi.eus/vamet/stations/stationList/stationList.json";
                var DeserialApiLocalidades = await LecturaApi(APILocalidades, _cliente);

                var balizas = PushLocalidades(DeserialApiLocalidades);


                /* cada vez que se extraigan los datos, habrá una espera de 10 minutos*/

                Thread.Sleep(countdown);
            }
        }

        /* Tarea para leer APIs*/
        static async Task<dynamic> LecturaApi(string url, HttpClient cliente)
        {
            HttpResponseMessage ResAPIurl = await cliente.GetAsync(url);
            var GetAPIurl = await ResAPIurl.Content.ReadAsStringAsync();
            dynamic urlDeserialJsonObj = JsonConvert.DeserializeObject(GetAPIurl);

            //var res = PushLocalidades(urlDeserialJsonObj);

            return urlDeserialJsonObj;

        }

        /* función para introducir localidades en la BDD y a su vez devuelve un array de balizas */

        static List<BalizLoc> PushLocalidades(dynamic datosLocali)
        {

            List<BalizLoc> Balizas = new List<BalizLoc>();

            /* recorremos todas las localidades */
            var i = 0;
            foreach (var localidad in datosLocali)
            {
                Console.Write("Provincia: " + localidad.province + "\n");
                Console.Write("Localidad: " + localidad.name + "\n");
                Console.Write("baliza: " + localidad.id + "\n");
                Console.Write("latitud (y): " + localidad.y + "\n");
                Console.Write("longitud (x): " + localidad.x + "\n");
                Console.Write("\n\n/=============/\n\n");
                Balizas.Add(new BalizLoc { Baliza = "Ball", Localidad = "Hello" });

            }

            return Balizas;
        }

        /* función que nitroduce los datos de las balizas en la BDD*/
    }
}
