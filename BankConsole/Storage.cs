using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankConsole;

public static class Storage{
    static string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\users.json";
    public static void AddUser(User user){
        string json = "", usersInFile = "";

        if(File.Exists(filePath)){
            usersInFile = File.ReadAllText(filePath);
        }
        var listUsers = JsonConvert.DeserializeObject<List<object>>(usersInFile);

        if(listUsers == null){
            listUsers = new List<object>();
        }

        listUsers.Add(user);

        JsonSerializerSettings settings = new JsonSerializerSettings{ Formatting = Formatting.Indented};

        json = JsonConvert.SerializeObject(listUsers,settings);
        File.WriteAllText(filePath, json);
    }
    public static List<User> GetUsers(){
        string usersInFile = "";
        var listUsers = new List<User>();
        if(File.Exists(filePath)){
            usersInFile = File.ReadAllText(filePath);
        }
        var listObjects = JsonConvert.DeserializeObject<List<object>>(usersInFile);

        if(listObjects == null){
            return listUsers;
        }
        foreach(object obj in listObjects){
            User newUser;
            JObject user = (JObject)obj;
            if(user.ContainsKey("TaxRegime")){
                newUser = user.ToObject<Client>();
            }else{
                newUser = user.ToObject<Employee>();
            }

            listUsers.Add(newUser);
        }
        return listUsers;
        
    }
    public static List<User> GetNewUsers(){
        var users = GetUsers();
        var newUsersList = users.Where(user => user.GetRegisterDate().Date.Equals(DateTime.Today)).ToList();

        return newUsersList;
    }

    public static string DeleteUser(int ID){
        var usersList = GetUsers();
        if(usersList.Count == 0){
            return "No hay usuarios en el archivo";
        }
        var userToDelete = usersList.Where(user => user.GetID() == ID).Single(); 
        usersList.Remove(userToDelete);
        JsonSerializerSettings settings = new JsonSerializerSettings{ Formatting = Formatting.Indented};

        string json = JsonConvert.SerializeObject(usersList,settings);
        File.WriteAllText(filePath, json);
        return "Success";
    }
    public static bool IdAlreadyExists(int id){
        var users = GetUsers();
        if(users.Count == 0){
            return false;
        }
        var duplicates = users.Where(user => user.GetID() == id);
        return duplicates.Count() > 0;
    }
}