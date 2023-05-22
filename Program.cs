using System;
using System.Net;

namespace DNSResolverApp
{
    class Program
    {
        static string dnsServer = Dns.GetHostName(); // DNS server implicit - cel indicat de sistem

        static void Main(string[] args)
        {
            Console.WriteLine("Aplicație de rezolvare DNS");

            while (true)
            {
                Console.WriteLine("Introduceți o comandă:");
                string command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command))
                    continue;

                string[] commandParts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string action = commandParts[0].ToLower();

                switch (action)
                {
                    case "resolve":
                        if (commandParts.Length < 2)
                        {
                            Console.WriteLine("Comanda 'resolve' necesită specificarea unui domeniu sau o adresă IP.");
                            continue;
                        }

                        string input = commandParts[1];

                        if (IsIpAddress(input))
                        {
                            string domain = GetDomainFromIpAddress(input);
                            Console.WriteLine($"Domeniul asociat adresei IP '{input}' este: {domain}");
                        }
                        else
                        {
                            IPAddress[] ipAddresses = GetIpAddressesFromDomain(input);
                            if (ipAddresses != null && ipAddresses.Length > 0)
                            {
                                Console.WriteLine($"Adresele IP asociate domeniului '{input}' sunt:");
                                foreach (IPAddress ipAddress in ipAddresses)
                                {
                                    Console.WriteLine(ipAddress.ToString());
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Nu s-au găsit adrese IP asociate domeniului '{input}'.");
                            }
                        }
                        break;

                    case "use":
                        if (commandParts.Length < 3 || commandParts[1].ToLower() != "dns")
                        {
                            Console.WriteLine("Comanda 'use dns' necesită specificarea unei adrese IP valide pentru DNS server.");
                            continue;
                        }

                        string newDnsServer = commandParts[2];
                        if (!IsIpAddress(newDnsServer))
                        {
                            Console.WriteLine("Adresa IP specificată pentru DNS server nu este validă.");
                            continue;
                        }

                        dnsServer = newDnsServer;
                        Console.WriteLine($"DNS server schimbat cu succes. Noul DNS server: {dnsServer}");
                        break;

                    default:
                        Console.WriteLine("Comandă nevalidă. Comenzile disponibile sunt 'resolve' și 'use dns'.");
                        break;
                }
            }
        }

        static bool IsIpAddress(string input)
        {
            if (IPAddress.TryParse(input, out IPAddress ipAddress))
            {
                return true;
            }
            return false;
        }

        static string GetDomainFromIpAddress(string ipAddress)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                return hostEntry.HostName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la rezolvarea domeniului pentru adresa IP '{ipAddress}': {ex.Message}");
                return string.Empty;
            }
        }

        static IPAddress[] GetIpAddressesFromDomain(string domain)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(domain);
                return hostEntry.AddressList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la rezolvarea adresei IP pentru domeniul '{domain}': {ex.Message}");
                return null;
            }
        }
    }
}
