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
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProcesoBackend
{
    class datosBaliza
    {
        public string idBaliza { get; set; }
        public string Localidad { get; set; }
        public string Estado { get; set; }
        public string Temperatura { get; set; }
        public string Humedad { get; set; }
        public string VelViento { get; set; }
    }

    class whoBaliza
    {
        public string idBaliza { get; set; }
        public string Localidad { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Comenzarem Prugrama");

            /* token para extraer los datos de las balizas*/

            const string token = "2f5f8474748c502e0906396fc69be329";

            /* cuenta atras de 10 minutos */

            TimeSpan countdown = new TimeSpan(0, 0, 600);

            /* Bucle para hacer la expropiación de datos infinitamente*/

            while (true)
            {
                /* añadimos un cliente para hacer la request a la API */

                var _cliente = new HttpClient();

                /* JWT */

                _cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                /* API localidades (euskalmet) */

                var APILocalidades = "https://www.euskalmet.euskadi.eus/vamet/stations/stationList/stationList.json";
                var DeserialApiLocalidades = await LecturaApi(APILocalidades, _cliente);

                Console.Write("Actualizando la base de datos\n");

                List<whoBaliza> balizas = PushLocalidades(DeserialApiLocalidades);
                

                /* isntanciando una lista para añadirle las localidades introducidas y luego eliminar las sobrantes de la bdd */

                List<string> areDupeBalizas = new List<string>();

                /* API balizas (openweather) */

                foreach (var baliza in balizas)
                {
                    var APIBalizas = $"http://api.openweathermap.org/data/2.5/weather?q={baliza.Localidad},ES&appid={token}";

                    try
                    {

                        var DeserialApiBalizas = await LecturaApi(APIBalizas, _cliente);

                        areDupeBalizas.Add(PushMediciones(DeserialApiBalizas, baliza));

                    }
                    catch (Exception awa)
                    {

                        Console.Write("Error, puede que esta baliza no contenga datos: " + awa + "\n");

                    }

                }

                /* Creando una lista de string sin localidades duplicadas */

                List<string> noDupeBalizas = areDupeBalizas;

                cleanDB(noDupeBalizas);

                Console.Write("Base de datos actualizada\n");

                /* cada vez que se extraigan los datos, habrá una espera de 10 minutos*/

                Thread.Sleep(countdown);
            }
        }

        /* Tarea para leer APIs*/
        static async Task<dynamic> LecturaApi(string url, HttpClient cliente)
        {
            try
            {

                HttpResponseMessage ResAPIurl = await cliente.GetAsync(url);
                var GetAPIurl = await ResAPIurl.Content.ReadAsStringAsync();
                dynamic urlDeserialJsonObj = JsonConvert.DeserializeObject(GetAPIurl);

                return urlDeserialJsonObj;

            }
            catch (Exception owo)
            {

                Console.Write("La API que intenta leer está vacía\n");

                return null;
            }
        }

        /* función para introducir localidades en la BDD y a su vez devuelve un array de balizas */

        static List<whoBaliza> PushLocalidades(dynamic datosLocali)
        {

            List<whoBaliza> Balizas = new List<whoBaliza>();

            /* recorremos todas las localidades */

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

                                var AddProvincia = new Provincias { Provincia = localidad.province };
                                db.Provincias.Add(AddProvincia);
                                db.SaveChanges();

                            }
                        }

                        /* comprobamos si alguna localidad está faltante (ya que hay más de 1 baliza por localidad, filtro los datos que me llegan por localidad) */

                        data = localidad.municipality;
                        DataExists = db.Localidades.Any(p => p.Localidad == data);

                        /* en caso de que falte la añadimos a la db */

                        if (localidad.province != "Navarra" && localidad.province != "Burgos")
                        {

                            data = localidad.id;

                            var isBaliza = db.Localidades.Any(p => p.idBaliza == data);

                            if(isBaliza){

                                /* Añadiendo los datos necesarios al array que devolvemos en la 2 iteración (cuando aún ya hay datos) */

                                 Balizas.Add(new whoBaliza{ idBaliza = localidad.id , Localidad = Convert.ToString(localidad.municipality)});

                            }

                            if (!DataExists)
                            {

                                /* Añadiendo los datos necesarios al array que devolvemos en la 1 iteración (cuando aún no hya datos) */

                                 Balizas.Add(new whoBaliza{ idBaliza = localidad.id , Localidad = Convert.ToString(localidad.municipality)});

                                var AddLocalidad = new Localidades { idBaliza = localidad.id, Localidad = localidad.municipality, Latitud = localidad.y, Longitud = localidad.x, Provincia = localidad.province };
                                db.Localidades.Add(AddLocalidad);
                                db.SaveChanges();



                            }
                        }

                    }

                }
                catch (Exception uwu)
                {

                    Console.Write("Ha ocurrido un error en la base de datos:" + uwu + "\nContacte con su proveedor por favor.\n");

                }
            }

            return Balizas;
        }

        /* función que introduce los datos de las balizas en la BDD */
        static string PushMediciones(dynamic datosBalizas, whoBaliza baliza)
        {

            var mediciones = new datosBaliza();

            mediciones.idBaliza = null;
            mediciones.Localidad = null;
            mediciones.Estado = null;
            mediciones.Temperatura = null;
            mediciones.VelViento = null;
            mediciones.Humedad = null;

            using (var db = new BaseTempoContext())
            {
                try
                {

                    foreach (var tipoMedicion in datosBalizas)
                    {

                        switch (tipoMedicion.Name.ToString())
                        {
                            case "weather":

                                /* Obtenemos el estado del cielo */

                                foreach (var dataWeather in tipoMedicion)
                                {

                                    /* estamos frente a un Jarray de objetos Jobject */

                                    if (dataWeather.Count > 1)
                                    {

                                        if (string.Equals(dataWeather[0]["main"].ToString(), "Mist", StringComparison.OrdinalIgnoreCase))
                                        {

                                            mediciones.Estado = dataWeather[1]["main"].ToString();
                                        }
                                        else
                                        {

                                            mediciones.Estado = dataWeather[0]["main"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        mediciones.Estado = dataWeather[0]["main"].ToString();
                                    }


                                }

                                break;

                            case "main":

                                // obtenemos la temperatura y humedad

                                foreach (var dataWeather in tipoMedicion)
                                {

                                    mediciones.Temperatura = dataWeather["temp"] - 273.15;

                                    mediciones.Humedad = dataWeather["humidity"];

                                }

                                break;

                            case "wind":

                                // obtenemos la velocidad del viento

                                foreach (var dataWeather in tipoMedicion)
                                {

                                    mediciones.VelViento = dataWeather["speed"];
                                }

                                break;
                        }

                    }


                    /* comprobamos si la baliza nos ha proporcionado todos los datos que queríamos */

                    if (!isEmpty(mediciones))
                    {

                        mediciones.idBaliza = baliza.idBaliza;
                        mediciones.Localidad = baliza.Localidad;

                        try
                        {


                            // comprobamos si existen datos de mediciones en la localidad en la BDD 


                            var DataExists = db.TemporalLocalidades.Any(tmp => tmp.idBaliza == baliza.idBaliza);


                            if (!DataExists)
                            {

                                var AddMediciones = new TemporalLocalidades { idBaliza = mediciones.idBaliza, Estado = mediciones.Estado, Temperatura = Math.Round(Convert.ToDouble(mediciones.Temperatura, System.Globalization.CultureInfo.InvariantCulture), 2), VelViento = Math.Round(Convert.ToDouble(mediciones.VelViento, System.Globalization.CultureInfo.InvariantCulture), 2), Humedad = Math.Round(Convert.ToDouble(mediciones.Humedad, System.Globalization.CultureInfo.InvariantCulture), 2) };
                                db.TemporalLocalidades.Add(AddMediciones);
                                db.SaveChanges();

                            }
                            else
                            {

                                var tupla = db.TemporalLocalidades.Where(tmp => tmp.idBaliza == baliza.idBaliza).Single();

                                tupla.Estado = mediciones.Estado;
                                tupla.Temperatura = Math.Round(Convert.ToDouble(mediciones.Temperatura, System.Globalization.CultureInfo.InvariantCulture), 2);
                                tupla.VelViento = Math.Round(Convert.ToDouble(mediciones.VelViento, System.Globalization.CultureInfo.InvariantCulture), 2);
                                tupla.Humedad = Math.Round(Convert.ToDouble(mediciones.Humedad, System.Globalization.CultureInfo.InvariantCulture), 2);

                                db.SaveChanges();
                            }

                            return mediciones.idBaliza;


                        }
                        catch (Exception iwi)
                        {
                            Console.Write("Ha ocurrido un error con la base de datos: " + iwi.InnerException.Message + "\n");
                        }



                    }
                    else
                    {
                        return baliza.idBaliza;
                        Console.Write("La baliza de " + baliza.Localidad + " (" + baliza.idBaliza + ") no tiene ninguna medición, puede que esté averiada\n");
                    }



                }
                catch (Exception ewe)
                {
                    Console.Write("Ha ocurrido un error:" + ewe + "\nContacte con su proveedor por favor.\n");
                }
            }

            /* si ha ocurrido algún error, este será el return */

            return "error";

        }

        /* función que elimina las localidades que no tengan mediciones */

        static void cleanDB(List<string> localidades)
        {


            using (var db = new BaseTempoContext())
            {
                try
                {

                    foreach (var localidad in localidades)
                    {

                        

                        var DataExists = db.TemporalLocalidades.Any(tmp => tmp.idBaliza == localidad);

                        if (!DataExists && localidad != "error")
                        {
                            var LocErrata = db.Localidades.Where(l => l.idBaliza == localidad).Single();
                            db.Localidades.Remove(LocErrata);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception a)
                {
                    Console.Write("Ha ocurrido un error a la hora de limpiar la base de datos" + a.InnerException.Message + "\n");
                }
            }

          

        }

        /* funcion que comprueba que la balizas tengan mediciones */
        static bool isEmpty(datosBaliza medicionesBaliza)
        {

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(medicionesBaliza)) // https://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
            {
                if (descriptor.GetValue(medicionesBaliza) != null)
                {
                    return false;
                }
            }
            return true;

        }

    }
}
