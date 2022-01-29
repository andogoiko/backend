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

    class BalizLoc
    {
        public string Baliza { get; set; }
        public string Localidad { get; set; }
    }

    class datosBaliza
    {
        public string Localidad { get; set; }
        public string velocidad { get; set; }
        public string velocidadMax { get; set; }
        public string temperatura { get; set; }
        public string humedad { get; set; }
        public string precipitacion { get; set; }
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

                Console.Write("Actualizando la base de datos\n");

                List<BalizLoc> balizas = PushLocalidades(DeserialApiLocalidades);

                /* API balizas */

                 foreach (var baliza in balizas)
                 {
                     var APIBalizas = $"https://www.euskalmet.euskadi.eus/vamet/stations/readings/{baliza.Baliza}/{getThisYear()}/{getThisMonth()}/{getToday()}/readingsData.json";
                     
                     try{

                        var DeserialApiBalizas = await LecturaApi(APIBalizas, _cliente);

                        var _baliza = new BalizLoc();

                        _baliza.Baliza = baliza.Baliza;
                        _baliza.Localidad = baliza.Localidad;

                        PushMediciones(DeserialApiBalizas, _baliza);

                     }catch(Exception awa){

                         Console.Write("Error, puede que esta baliza no contenga datos: " + awa + "\n");

                     }

                 }

                /* Creando una lista de string sin localidades duplicadas */

                List<string> noDupeBalizas = balizas.Select(loc => loc.Localidad)
                                            .Distinct()
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

        static List<BalizLoc> PushLocalidades(dynamic datosLocali)
        {

            List<BalizLoc> Balizas = new List<BalizLoc>();

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

                            Balizas.Add(new BalizLoc { Baliza = $"{localidad.id}", Localidad = $"{localidad.municipality}" });

                            if (!DataExists)
                            {

                                var AddLocalidad = new Localidades { Localidad = localidad.municipality, Baliza = localidad.id, Latitud = localidad.y, Longitud = localidad.x, Provincia = localidad.province };
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
        static void PushMediciones(dynamic datosBalizas, BalizLoc baliza)
        {

            var mediciones = new datosBaliza();

            mediciones.Localidad = null;
            mediciones.temperatura = null;
            mediciones.velocidad = null;
            mediciones.velocidadMax = null;
            mediciones.humedad = null;
            mediciones.precipitacion = null;

            using (var db = new BaseTempoContext())
            {
                try{

                    foreach (var tipoMedicion in datosBalizas)
                    {
                        foreach (var tipo in tipoMedicion)
                        {

                            /* recogemos el objeto que contiene los datos de unas mediciones específicas (puede ser temperatura, viento, etc según la iteración del foreach)*/

                            JObject objDatosHora = JObject.Parse(tipo["data"].ToString());

                            /* Recogemos las propiedades que tiene este objeto en una lista de strings*/

                            IList<string> propiedades = objDatosHora.Properties().Select(p => p.Name).ToList(); // https://www.newtonsoft.com/json/help/html/M_Newtonsoft_Json_Linq_JObject_Properties.htm

                            /* recogemos la 1ª propiedad en un objeto, que es la que contiene las mediciones */

                            JObject JsonMediciones = JObject.Parse(objDatosHora[propiedades[0]].ToString());

                            /* convertimos a lista el objeto con las mediciones */

                            List<string> lMediciones = JsonMediciones.Properties().Select(p => p.Name).ToList();

                            /* ordenamos la lista */

                            lMediciones.Sort();

                            /* guardando los datos */

                            switch (tipo.name.ToString()) 
                            {
                                case "mean_speed":

                                    /* obtenemos la última medición de la media del viento y guardamos el dato */
                                    
                                    var avgSpd = Convert.ToString(JsonMediciones[lMediciones.Last()]);

                                    mediciones.velocidad = avgSpd;
                                    
                                    
                                    break;
                                case "max_speed":
                                    
                                    /* obtenemos la última medición de la velocidad máxima del viento y guardamos el dato */

                                    var MaxSpd = Convert.ToString(JsonMediciones[lMediciones.Last()]);

                                    mediciones.velocidadMax = MaxSpd;

                                    break;
                                case "temperature":
                                    
                                    /* obtenemos la última medición de la temperatura y guardamos el dato */
                                    
                                    var temp = Convert.ToString(JsonMediciones[lMediciones.Last()]);

                                    mediciones.temperatura = temp;

                                    break;
                                case "humidity":
                                    
                                    /* obtenemos la última medición de la humedad y guardamos el dato */
                                    
                                    var wet = Convert.ToString(JsonMediciones[lMediciones.Last()]);

                                    mediciones.humedad = wet;

                                    break;
                                case "precipitation":

                                    /* obtenemos la última medición de las precipitaciones y guardamos el dato */
                                    
                                    var precipitation = Convert.ToString(JsonMediciones[lMediciones.Last()]);

                                    mediciones.precipitacion = precipitation;

                                    break;
                            }
                            
                            
                        }
                        
                    }

                    
                    /* comprobamos si la baliza nos ha proporcionado todos los datos que queríamos */

                    if(!isEmpty(mediciones)){

                        mediciones.Localidad = baliza.Localidad;

                            try{


                                /* comprobamos si existen datos de mediciones en la localidad en la BDD */

                                var DataExists = db.TemporalLocalidades.Any(tmp => tmp.Localidad == baliza.Localidad);


                                if(!DataExists){

                                    var AddMediciones = new TemporalLocalidades { Localidad = mediciones.Localidad, Temperatura = Convert.ToDouble(mediciones.temperatura), VelViento = Convert.ToDouble(mediciones.velocidad), VelVientoMax = Convert.ToDouble(mediciones.velocidadMax), Precipitaciones = Convert.ToDouble(mediciones.precipitacion), Humedad =  Convert.ToDouble(mediciones.humedad)};
                                    db.TemporalLocalidades.Add(AddMediciones);
                                    db.SaveChanges(); 

                                }else{

                                    var tupla = db.TemporalLocalidades.Where(tmp => tmp.Localidad == baliza.Localidad).Single();

                                    tupla.Temperatura = Convert.ToDouble(mediciones.temperatura);
                                    tupla.VelViento = Convert.ToDouble(mediciones.velocidad);
                                    tupla.VelVientoMax = Convert.ToDouble(mediciones.velocidadMax);
                                    tupla.Precipitaciones = Convert.ToDouble(mediciones.precipitacion);
                                    tupla.Humedad = Convert.ToDouble(mediciones.humedad);

                                    db.SaveChanges(); 
                                }

                                

                            }catch(Exception iwi){
                                Console.Write("Ha ocurrido un error con la base de datos: " + iwi.InnerException.Message + "\n");
                            }

                            

                        } else{

                            Console.Write("La baliza de " + baliza.Baliza + " no tiene ninguna medición, puede que esté averiada\n");
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

        /* función que devuelve el día de hoy */
        static string getToday(){
            DateTime Hoy = DateTime.Today;

            if(Hoy.Day.ToString().Length == 1){

                return "0" + Hoy.Day.ToString();
            }

            return Hoy.Day.ToString();
        }

        /* función que devuelve el mes actual */

        static string getThisMonth(){
            DateTime Hoy = DateTime.Today;

            if(Hoy.Month.ToString().Length == 1){

                return "0" + Hoy.Month.ToString();
            }

            return Hoy.Month.ToString();
        }

        /* función que que devuelve el año actual */
        static string getThisYear(){
            DateTime Hoy = DateTime.Today;

            return Hoy.Year.ToString();
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
