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
        public string Localidad { get; set; }
        public string Estado { get; set; }
        public string Temperatura { get; set; }
        public string Humedad { get; set; }
        public string VelViento { get; set; }
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

                List<String> balizas = PushLocalidades(DeserialApiLocalidades);

                /* API balizas (openweather) */

                 foreach (var baliza in balizas)
                 {
                     var APIBalizas = $"http://api.openweathermap.org/data/2.5/weather?q={baliza}&appid={token}";
                     
                     try{

                        var DeserialApiBalizas = await LecturaApi(APIBalizas, _cliente);

                        PushMediciones(DeserialApiBalizas, baliza);

                     }catch(Exception awa){

                         Console.Write("Error, puede que esta baliza no contenga datos: " + awa + "\n");

                     }

                 }

                /* Creando una lista de string sin localidades duplicadas */

                List<string> noDupeBalizas = balizas.Distinct()
                                            .ToList();

                cleanDB(noDupeBalizas);

                Console.Write("Base de datos actualizada\n");

                /* cada vez que se extraigan los datos, habrá una espera de 10 minutos*/

                Thread.Sleep(countdown);
            }
        }

        /* Tarea para leer APIs*/
        static async Task<dynamic> LecturaApi(string url, HttpClient cliente)
        {
            try{

                HttpResponseMessage ResAPIurl = await cliente.GetAsync(url);
                var GetAPIurl = await ResAPIurl.Content.ReadAsStringAsync();
                dynamic urlDeserialJsonObj = JsonConvert.DeserializeObject(GetAPIurl);

                return urlDeserialJsonObj;

            }catch(Exception owo){

                Console.Write("La API que intenta leer está vacía\n");

                return null;
            }
        }

        /* función para introducir localidades en la BDD y a su vez devuelve un array de balizas */

        static List<String> PushLocalidades(dynamic datosLocali)
        {

            List<String> Balizas = new List<String>();

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

                        /* comprobamos si alguna localidad está faltante */

                        data = localidad.municipality;
                        DataExists = db.Localidades.Any(p => p.Localidad == data);

                        /* en caso de que falte la añadimos a la db */

                        if (localidad.province != "Navarra" && localidad.province != "Burgos")
                        {

                            /* Añadiendo los datos necesarios al array que devolvemos */

                            Balizas.Add(Convert.ToString(localidad.municipality));

                            if (!DataExists)
                            {

                                var AddLocalidad = new Localidades { Localidad = localidad.municipality, Latitud = localidad.y, Longitud = localidad.x, Provincia = localidad.province };
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
        static void PushMediciones(dynamic datosBalizas, string baliza)
        {

            var mediciones = new datosBaliza();

            mediciones.Localidad = null;
            mediciones.Estado = null;
            mediciones.Temperatura = null;
            mediciones.VelViento = null;
            mediciones.Humedad = null;

            using (var db = new BaseTempoContext())
            {
                try{

                    foreach (var tipoMedicion in datosBalizas)
                    {

                        switch (tipoMedicion.Name.ToString()) 
                            {
                                case "weather":

                                    /* Obtenemos el estado del cielo */

                                    foreach (var dataWeather in tipoMedicion)
                                    {

                                        /* estamos frente a un Jarray de objetos Jobject */

                                        if(dataWeather.Count > 1){

                                            if(string.Equals(dataWeather[0]["main"].ToString() , "Mist", StringComparison.OrdinalIgnoreCase)){

                                                mediciones.Estado = dataWeather[1]["main"].ToString();
                                            }else{

                                                mediciones.Estado = dataWeather[0]["main"].ToString();
                                            }
                                        }else{
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

                    if(!isEmpty(mediciones)){

                        mediciones.Localidad = baliza;

                            try{


                                // comprobamos si existen datos de mediciones en la localidad en la BDD 

                                var DataExists = db.TemporalLocalidades.Any(tmp => tmp.Localidad == baliza);


                                if(!DataExists){

                                    var AddMediciones = new TemporalLocalidades { Localidad = mediciones.Localidad, Estado = mediciones.Estado, Temperatura = Math.Round(Convert.ToDouble(mediciones.Temperatura, System.Globalization.CultureInfo.InvariantCulture), 2), VelViento = Math.Round(Convert.ToDouble(mediciones.VelViento, System.Globalization.CultureInfo.InvariantCulture), 2), Humedad =  Math.Round(Convert.ToDouble(mediciones.Humedad, System.Globalization.CultureInfo.InvariantCulture), 2)};
                                    db.TemporalLocalidades.Add(AddMediciones);
                                    db.SaveChanges(); 

                                }else{

                                    var tupla = db.TemporalLocalidades.Where(tmp => tmp.Localidad == baliza).Single();

                                    tupla.Estado = mediciones.Estado;
                                    tupla.Temperatura = Math.Round(Convert.ToDouble(mediciones.Temperatura, System.Globalization.CultureInfo.InvariantCulture), 2);
                                    tupla.VelViento = Math.Round(Convert.ToDouble(mediciones.VelViento, System.Globalization.CultureInfo.InvariantCulture), 2);
                                    tupla.Humedad = Math.Round(Convert.ToDouble(mediciones.Humedad, System.Globalization.CultureInfo.InvariantCulture), 2);

                                    db.SaveChanges(); 
                                }

                                

                            }catch(Exception iwi){
                                Console.Write("Ha ocurrido un error con la base de datos: " + iwi.InnerException.Message + "\n");
                            }

                            

                        } else{

                            Console.Write("La baliza de " + baliza + " no tiene ninguna medición, puede que esté averiada\n");
                    }
                    
                    

                }catch(Exception ewe){
                    Console.Write("Ha ocurrido un error:" + ewe + "\nContacte con su proveedor por favor.\n");
                }
            }

        }

        /* función que elimina las localidades que no tengan mediciones */

        static void cleanDB(List<string> localidades){
            

            using (var db = new BaseTempoContext()){
                try{

                    foreach (var localidad in localidades)
                    {
                        
                        var DataExists = db.TemporalLocalidades.Any(tmp => tmp.Localidad == localidad);

                        if(!DataExists){
                         
                            var LocErrata = db.Localidades.Where(l => l.Localidad == localidad).Single();
                            db.Localidades.Remove(LocErrata);
                            db.SaveChanges();
                        }
                    }
                }catch(Exception a){
                    Console.Write("Ha ocurrido un error a la hora de limpiar la base de datos" + a.InnerException.Message + "\n");
                }
            }
            
        }

        /* funcion que comprueba que la balizas tengan mediciones */
        static bool isEmpty(datosBaliza medicionesBaliza){

            foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(medicionesBaliza)) // https://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
            {
                if(descriptor.GetValue(medicionesBaliza) != null){
                    return false;
                }
            }
            return true;
            
        }

    }
}
