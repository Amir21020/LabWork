using System.Globalization;
using System.Text;

namespace DataTypes.Task1;

public static class Calculator
{
    public static string CalculateCompoundInterest(decimal initialDeposit, int years, decimal interestRate)
    {
        StringBuilder result = new();
        decimal balance = initialDeposit;
        for (int year = 1; year <= years; year++)
        {
            balance += (balance * interestRate) / 100;

            string formattedBalance = balance.ToString("F2", CultureInfo.InvariantCulture);
            result.AppendLine($"Год {year}: {formattedBalance} руб.");
        }
        return result.ToString().TrimEnd();
    }
}