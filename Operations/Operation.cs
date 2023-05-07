namespace EventApp.Operations;
public class Operation
{
    private static readonly Random random = new Random();
    public static string GetTicketNumber()
    {
        var charList = new List<char>(){'A','E','U','K','D','S','N'};
        int tno1 = random.Next(0,charList.Count);
        int tno2 = random.Next(0,charList.Count);
        int tnnumeric = random.Next(100000,999999);
        string TicketNo = String.Concat(charList[tno1],charList[tno2],tnnumeric);
        return TicketNo;
    }
}
