using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace Restaurant.PL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPLServices(this IServiceCollection services,
                                                       IConfiguration configuration,
                                                       IWebHostEnvironment environment)
        {
            string owlFilePath = Path.Combine(environment.WebRootPath, "Res-SWprojectFinal.xml");

            IGraph graph = new Graph();

            try
            {
                RdfXmlParser parser = new RdfXmlParser();
                parser.Load(graph, owlFilePath);


                SaveTriplesToDatabase(graph, configuration);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("problem with loading the file");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }

            ProcessIndividuals(graph);

            SaveIndividualsToDatabase(graph, configuration);

            return services;
        }

        private static void SaveTriplesToDatabase(IGraph graph, IConfiguration configuration)
        {
            // Database connection string
            string connectionString = configuration.GetConnectionString("DefaultConnection")!;

            string insertSql = "INSERT INTO RdfTriples (Subject, Predicate, Object) VALUES (@subject, @predicate, @object)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (Triple triple in graph.Triples)
                {
                    string subject = triple.Subject.ToString();
                    string predicate = triple.Predicate.ToString();
                    string obj = triple.Object.ToString();

                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@subject", subject);
                        command.Parameters.AddWithValue("@predicate", predicate);
                        command.Parameters.AddWithValue("@object", obj);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void ProcessIndividuals(IGraph graph)
        {
            Console.WriteLine("Extracting Individuals and their Properties...");

            // Iterate through all triples in the graph
            foreach (Triple triple in graph.Triples)
            {
                // Check if the subject is a NamedIndividual
                if (triple.Subject is IUriNode subjectNode && triple.Predicate.ToString().Contains("type"))
                {
                    Console.WriteLine($"Individual: {subjectNode.Uri}");

                    // Check if it's linked to a class
                    if (triple.Object is IUriNode objectNode)
                    {
                        Console.WriteLine($" - Belongs to Class: {objectNode.Uri}");
                    }
                }

                // Handle properties of the individual
                if (triple.Predicate is IUriNode predicateNode && triple.Object is ILiteralNode literalNode)
                {
                    Console.WriteLine($" - Property: {predicateNode.Uri}, Value: {literalNode.Value}");
                }
            }
        }

        private static void SaveIndividualsToDatabase(IGraph graph, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")!;
            string insertSql = "INSERT INTO RdfIndividuals (Individual, Class, Property, Value) VALUES (@individual, @class, @property, @value)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (Triple triple in graph.Triples)
                {
                    string? individual = null, individualClass = null, property = null, value = null;

                    // Check for type relationships
                    if (triple.Predicate.ToString().Contains("type") && triple.Subject is IUriNode subjectNode)
                    {
                        individual = subjectNode.Uri.ToString();
                        individualClass = triple.Object.ToString();
                    }

                    // Check for properties
                    if (triple.Predicate is IUriNode predicateNode && triple.Object is ILiteralNode literalNode)
                    {
                        property = predicateNode.Uri.ToString();
                        value = literalNode.Value;
                    }

                    // Insert if it's an individual or property
                    if (individual != null || property != null)
                    {
                        using (SqlCommand command = new SqlCommand(insertSql, connection))
                        {
                            command.Parameters.AddWithValue("@individual", individual ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@class", individualClass ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@property", property ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@value", value ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

    }
}
