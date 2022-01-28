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

                /* API localidades */

                var APILocalidades = "https://www.euskalmet.euskadi.eus/vamet/stations/stationList/stationList.json";
                var DeserialApiLocalidades = await LecturaApi(APILocalidades, _cliente);

                var balizas = PushLocalidades(DeserialApiLocalidades);

                /* API balizas */


                /* foreach (var baliza in balizas)
                 {
                     var APIBalizas = $"https://www.euskalmet.euskadi.eus/vamet/stations/readings/{baliza.Baliza}/2022/01/22/readingsData.json";
                     //var DeserialApiBalizas = await LecturaApi(APIBalizas, _cliente);

                 }*/

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

            using (var db = new BaseTempoContext())
            {
                try
                {

                    foreach (var localidad in datosLocali)
                    {

                        /* comprobamos si alguna provincia está faltante */

                        string data = localidad.province;
                        var DataExists = db.Provincias.Any(p => p.Provincia == data);

                        /* en caso de que falte la añadimos a la db */

                        if (!DataExists)
                        {
                            if (data != "Navarra" && data != "Burgos")
                            {
                                Console.Write("Actualizando la base de datos\n");

                                var AddProvincia = new Provincias { Provincia = localidad.province };
                                db.Provincias.Add(AddProvincia);
                                db.SaveChanges();

                                Console.Write("Base de datos actualizada\n");
                            }
                        }

                        /* comprobamos si alguna localidad está faltante */

                        data = localidad.municipality;
                        DataExists = db.Localidades.Any(p => p.Localidad == data);

                        /* en caso de que falte la añadimos a la db */

                        if (!DataExists)
                        {

                            if (localidad.province != "Navarra" && localidad.province != "Burgos")
                            {
                                Console.Write("Actualizando la base de datos\n");

                                var AddLocalidad = new Localidades { Localidad = localidad.municipality, Baliza = localidad.id, Latitud = localidad.y, Longitud = localidad.x, Provincia = localidad.province };
                                db.Localidades.Add(AddLocalidad);
                                db.SaveChanges();

                                Console.Write("Base de datos actualizada\n");

                            }
                        }

                        /* Añadiendo los datos necesarios al array que devolvemos */

                        Balizas.Add(new BalizLoc { Baliza = $"{localidad.id}", Localidad = $"{localidad.name}" });


                    }

                }
                catch (Exception uwu)
                {
                    Console.Write("Ha ocurrido un error, contacte con su proveedor por favor.");
                }
            }

            return Balizas;
        }

        /* función que introduce los datos de las balizas en la BDD*/
        static void PushTemp(dynamic datosBalizas)
        {


        }
    }
}
