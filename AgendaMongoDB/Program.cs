using AgendaMongoDB; 
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace AgendaMongoDBApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Conexion MongoDB
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("agenda");
            IMongoCollection<Contacto> collection = database.GetCollection<Contacto>("contactos");

            while (true)
            {
                Console.WriteLine("Seleccione una opción:");
                Console.WriteLine("1. Mostrar personas por letra");
                Console.WriteLine("2. Mostrar contactos por ciudad");
                Console.WriteLine("3. Mostrar contactos por estado");
                Console.WriteLine("4. Actualizar número de teléfono");
                Console.WriteLine("5. Agregar nuevo contacto");
                Console.WriteLine("6. Eliminar contactos por estado");
                Console.WriteLine("7. Salir");
              
                int option;
                if (int.TryParse(Console.ReadLine(), out option))
                {
                    switch (option)
                    {
                        case 1:
                            ShowPeopleByLetter(collection);
                            break;
                        case 2:
                            ShowContactsByCountry(collection);
                            break;
                        case 3:
                            ShowContactsByState(collection);
                            break;
                        case 4:
                            UpdatePhoneNumber(collection);
                            break;
                        case 5:
                            AddNewContact(collection); 
                            break;
                        case 6:
                            DeleteContactsByState(collection);
                            break;
                        case 7:
                            Console.WriteLine("Saliendo del programa.");
                            return;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Entrada no válida. Ingrese un número.");
                }
            }
        }

        // Muestra los contactos por letra
        static void ShowPeopleByLetter(IMongoCollection<Contacto> collection)
        {
            Console.WriteLine("Ingrese la letra para la búsqueda:");
            string searchLetter = Console.ReadLine();
            FilterDefinition<Contacto> filter = Builders<Contacto>.Filter.Regex("Name", new BsonRegularExpression($"^{searchLetter}", "i"));
            List<Contacto> matchingContacts = collection.Find(filter).ToList();

            foreach (var contacto in matchingContacts)
            {
                Console.WriteLine($"{contacto.Name}");
            }
        }

        // Muestra los contactos por pais
        static void ShowContactsByCountry(IMongoCollection<Contacto> collection)
        {
            Console.WriteLine("Ingrese el país para la búsqueda:");
            string targetCountry = Console.ReadLine();
            FilterDefinition<Contacto> countryFilter = Builders<Contacto>.Filter.Eq("country", targetCountry);
            SortDefinition<Contacto> sortByName = Builders<Contacto>.Sort.Ascending("name");
            List<Contacto> countryContacts = collection.Find(countryFilter).Sort(sortByName).ToList();

            // Para evitar los duplicados
            HashSet<string> uniqueContacts = new HashSet<string>(); 

            foreach (var contacto in countryContacts)
            {
                string contactInfo = $"{contacto.Name} - {contacto.Country}";
                if (!uniqueContacts.Contains(contactInfo))
                {
                    Console.WriteLine(contactInfo);
                    uniqueContacts.Add(contactInfo);
                }
            }
        }

        // Muestra los contactos por estado
        static void ShowContactsByState(IMongoCollection<Contacto> collection)
        {
            Console.WriteLine("Ingrese el estado para la búsqueda:");
            string targetState = Console.ReadLine();
            FilterDefinition<Contacto> stateFilter = Builders<Contacto>.Filter.Eq("state", targetState); // Utiliza el campo "state"
            SortDefinition<Contacto> sortByName = Builders<Contacto>.Sort.Ascending("name");
            List<Contacto> stateContacts = collection.Find(stateFilter).Sort(sortByName).ToList();

            foreach (var contacto in stateContacts)
            {
                Console.WriteLine($"{contacto.Name} - {contacto.State}");
            }
        }

        // Actualiza los datos del contacto como el nombre y el telefono
        static void UpdatePhoneNumber(IMongoCollection<Contacto> collection)
        {
            Console.WriteLine("Ingrese el nombre del contacto a actualizar:");
            string targetName = Console.ReadLine();
            Console.WriteLine("Ingrese el nuevo número de teléfono:");
            string newPhoneNumber = Console.ReadLine();

            FilterDefinition<Contacto> nameFilter = Builders<Contacto>.Filter.Eq("Name", targetName);
            UpdateDefinition<Contacto> updatePhone = Builders<Contacto>.Update.Set("PhoneNumber", newPhoneNumber);
            UpdateResult updateResult = collection.UpdateOne(nameFilter, updatePhone);

            Console.WriteLine($"Actualizados {updateResult.ModifiedCount} contacto(s).");
        }

        // Agrega un nuevo contacto
        static void AddNewContact(IMongoCollection<Contacto> collection)
        {
            Contacto newContact = new Contacto();

            Console.WriteLine("Ingrese el nombre:");
            newContact.Name = Console.ReadLine();

            Console.WriteLine("Ingrese la fecha de nacimiento (dd/mm/aaaa):");
            if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime birthDate))
            {
                newContact.DateBirth = birthDate;
            }
            else
            {
                Console.WriteLine("Formato de fecha inválido. Se usará la fecha mínima.");
            }

            Console.WriteLine("Ingrese el país:");
            newContact.Country = Console.ReadLine();

            Console.WriteLine("Ingrese el estado:");
            newContact.State = Console.ReadLine();

            Console.WriteLine("Ingrese la dirección:");
            newContact.Address = Console.ReadLine();

            Console.WriteLine("Ingrese el correo electrónico:");
            newContact.Email = Console.ReadLine();

            Console.WriteLine("Ingrese el número de teléfono:");
            newContact.Phone = Console.ReadLine();

            collection.InsertOne(newContact);
            Console.WriteLine("Contacto agregado exitosamente.");
        }

    // Elimina un contacto mediante el nombre del contacto
    static void DeleteContactsByState(IMongoCollection<Contacto> collection)
        {
            Console.WriteLine("Ingrese el estado para eliminar contactos:");
            string stateToDelete = Console.ReadLine();
            FilterDefinition<Contacto> stateFilter = Builders<Contacto>.Filter.Eq("State", stateToDelete);
            DeleteResult deleteResult = collection.DeleteMany(stateFilter);

            Console.WriteLine($"Eliminados {deleteResult.DeletedCount} contacto(s) de {stateToDelete}.");
        }

    }
}


//Insertar Datos (JSON Y CSV)

//string directorio = $"{Environment.CurrentDirectory}\\Data";
//try
//{
//    if (!Directory.Exists(directorio))
//    {
//        throw new Exception($"El directorio {directorio} no existe");
//    }

//    string[] archivos = Directory
//        .GetFiles(directorio)
//        .AsEnumerable()
//        .Where(a => a.EndsWith(".csv") || a.EndsWith(".json")) 
//        .ToArray();

//    foreach (var archivo in archivos)
//    {
//        //Console.WriteLine(archivo); 
//        InsertarJson(archivo);
//        InsertarCsv(archivo);
//    }
//}
//catch (Exception ex)
//{

//    Console.WriteLine(ex.Message);

//}

//List<Contacto> InsertarCsv(string archivo)
//{
//    List<Contacto> contactos = new List<Contacto>();
//    if (Path.GetExtension(archivo) == ".csv")
//    {
//        string[] csv = File.ReadAllLines(archivo);

//        foreach (string renglon in csv)
//        {
//            if (!renglon.Contains("Name,DateBirth") && !string.IsNullOrWhiteSpace(renglon))
//            {
//                //Expresion Regular!
//                string[] datos = Regex.Split(renglon, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

//                Contacto contacto = new Contacto();
//                contacto.Name = datos[0].Corregir();
//                contacto.DateBirth = Convert.ToDateTime(datos[1].Corregir());
//                contacto.Country = datos[2].Corregir();
//                contacto.State = datos[3].Corregir();
//                contacto.Address = datos[4].Corregir();
//                contacto.Email = datos[5].Corregir();
//                contacto.Phone = datos[6].Corregir();
//                contactos.Add(contacto);
//            }
//        }
//        collection.InsertMany(contactos);
//    }

//    Console.WriteLine("Proceso Terminado");
//    return new List<Contacto>();
//}

//List<Contacto> InsertarJson(string archivo)
//{
//    List<Contacto> contactos = new List<Contacto>();
//    if (Path.GetExtension(archivo) == ".json")
//    {
//        string json = File.ReadAllText(archivo);
//        contactos = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonData>(json)!.objects;
//        collection.InsertMany(contactos);
//    }

//    Console.WriteLine("Proceso Terminado");
//    return new List<Contacto>();
//}













//Contacto contacto= new Contacto();
//contacto.Name = "Putter";
//contacto.Address = "9 3/4";
//contacto.Phone= "1234567890";

////Insert(contacto);


//Contacto contacto2 = new Contacto();
//contacto2.Name = "Sasuke";
//contacto2.Address = "Konoha";
//contacto2.Phone= "1234567890";

////Insert(contacto2);

////Contacto contactoEncontrado = FindByName("Putter Virginio")!;
////Console.WriteLine(contactoEncontrado!.ToString());


//Contacto contactoActualizado = new Contacto()
//{
//    Id = contactoEncontrado.Id,
//    Name = "Putter Virginio",
//    Phone = "234567890",
//};

//Update(contactoEncontrado.Id, contactoActualizado);
//contactoEncontrado = FindById(contactoEncontrado.Id);
//Console.WriteLine(contactoEncontrado.ToString());

//Delete(contactoEncontrado.Id);


//Console.WriteLine( );


//foreach (Contacto c in Get())
//{
//    Console.WriteLine(c.ToString());
//}

//void Insert(Contacto contacto)
//{
//    collection.InsertOne(contacto);
//}

//Contacto FindByName(string name)
//{
//    return collection.Find(c => c.Name == name)
//        .FirstOrDefault<Contacto>();
//}

//Contacto FindById(string id)
//{
//    return collection.Find(c => c.Id == id)
//        .FirstOrDefault<Contacto>();
//}

//List<Contacto> Get()
//{
//    return collection.Find(c => true).ToList();
//}

//void Update(string id, Contacto contacto)
//{
//    Contacto ContactoActualizar = FindById(id);
//    if (ContactoActualizar != null)
//    {
//        collection.ReplaceOne(c => c.Id == ContactoActualizar.Id, contacto);
//    }
//}

//void Delete(string id)
//{
//    Contacto contacto = FindById(id);
//    if (contacto != null)
//    {
//        collection.DeleteOne(c => c.Id == id);
//    }
//}


