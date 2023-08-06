namespace BankConsole;

public abstract class Person{
    public abstract string GetName();

    public string GetCountry(){
        return "Germany";
    }
}

public interface IPerson{
    string GetName();
    string GetCountry();
}