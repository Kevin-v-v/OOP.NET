using System.Text.RegularExpressions;
using BankConsole;

if(args.Length == 0){
    await EmailService.SendMail();
}else{
    ShowMenu();
}

void ShowMenu(){
    Console.Clear();
    Console.WriteLine(
@"Selecciona una opción:
1 - Crear un Usuario nuevo.
2 - Eliminar un Usuario existente.
3 - Salir.
");
    int option = 0;
    do
    {
        string input = Console.ReadLine();
        if(!int.TryParse(input, out option)){
            Console.WriteLine("Debes ingresar un número (1, 2, o 3).");
        }else if(option < 1 || option > 3)
        {
            Console.WriteLine("Debes ingresar un número válido (1, 2, o 3).");
        }
    } while (option < 1 || option > 3);

    switch (option)
    {
        case 1:
            CreateUser();
        break;
        case 2:
            DeleteUser();
        break;
        case 3:
            Environment.Exit(0);
        break;
    }

}


void CreateUser()
{
    int ID;
    string name = "", email = "", department = "", input = "";
    decimal balance;
    char userType, taxRegime;
    bool invalidInput;
    Regex emailRegExp = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9]+(\.[a-zA-Z0-9]+){1,2}$");
    Console.Clear();
    Console.WriteLine("Ingresa la información del usuario:");

    Console.WriteLine("ID: ");
    invalidInput = true;
    do
    {
        input = Console.ReadLine();
        if(!int.TryParse(input, out ID)){
            Console.WriteLine("Debes ingresar un número entero positivo.");
        }else if(Storage.IdAlreadyExists(ID))
        {
            Console.WriteLine($"El ID \"{ID}\" ya existe, intente otro.");
        }else{
            invalidInput = false;
        }
    } while (invalidInput);

    Console.WriteLine("Nombre: ");
    name = Console.ReadLine();

    Console.WriteLine("Email: ");
    invalidInput = true;
    do
    {
        input = Console.ReadLine();
        if(!emailRegExp.IsMatch(input))
        {
            Console.WriteLine($"El Email debe ser de la forma alguien@ejemplo.com. Ingrese un Email válido.");
        }else{
            invalidInput = false;
            email = input;
        }
    } while (invalidInput);
    
    Console.WriteLine("Saldo: ");
    invalidInput = true;

    do
    {
        input = Console.ReadLine();
        if(!decimal.TryParse(input, out balance)){
            Console.WriteLine("Debes ingresar un número decimal positivo.");
        }else if(balance <= 0){
            Console.WriteLine("Debes ingresar un número decimal positivo.");
        }else{
            invalidInput = false;
        }
    } while (invalidInput);

    Console.WriteLine("Escribe 'c' si el usuario es Cliente, 'e' si es Empleado: ");
    invalidInput = true;

    do
    {
        input = Console.ReadLine();
        if(!char.TryParse(input, out userType)){
            Console.WriteLine("Debes ingresar 'c' si el usuario es Cliente, 'e' si es Empleado. Intenta de nuevo.");
        }else if(userType != 'c' && userType != 'e'){
            Console.WriteLine("Debes ingresar 'c' si el usuario es Cliente, 'e' si es Empleado. Intenta de nuevo.");
        }else{
            invalidInput = false;
        }
    } while (invalidInput);

    User user = null;

    switch(userType){
        case 'c':
            Console.WriteLine("Regimen Fiscal: ");
            invalidInput = true;
            do{
                input = Console.ReadLine();
                if(!char.TryParse(input, out taxRegime)){
                    Console.WriteLine("Debes ingresar un solo caracter.");
                }else{
                    invalidInput = false;
                }
            } while (invalidInput);
            user = new Client(ID, name, email, balance, taxRegime);
        break;
        case 'e':
            Console.WriteLine("Departamento: ");
            department = Console.ReadLine();
            user = new Employee(ID, name, email, balance, department);
        break;
    }
    Storage.AddUser(user);
    Console.WriteLine("Usuario creado.");
    Thread.Sleep(2000);
    ShowMenu();
}
void DeleteUser()
{
    bool invalidInput = true;
    string input;
    int ID;
    Console.Clear();
    Console.WriteLine("Ingresa el ID del usuario a eliminar:");

    Console.WriteLine("ID: ");
    
    do
    {
        input = Console.ReadLine();
        if(!int.TryParse(input, out ID)){
            Console.WriteLine("Debes ingresar un número entero positivo.");
        }else if(!Storage.IdAlreadyExists(ID))
        {
            Console.WriteLine($"El ID \"{ID}\" no existe, intente otro.");
        }else{
            invalidInput = false;
        }
    } while (invalidInput);
    string result = Storage.DeleteUser(ID);
    if(result.Equals("Success")){
        Console.WriteLine("Usuario eliminado.");
    }else{
        Console.WriteLine("Ocurrió un error. " + result);
    }
    Thread.Sleep(2000);
    ShowMenu();
}

