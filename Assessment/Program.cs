using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ArtGalleryApp
{
    class Program
    {
        private static string connectionString =
            "Server=localhost,1433;Database=College;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            int choice;
            do
            {
                Console.Clear();
                Console.WriteLine("---===--- Art Gallery Management ---===---");
                Console.WriteLine("1. Insert Artist");
                Console.WriteLine("2. Insert Artwork");
                Console.WriteLine("3. View Artworks with Artist");
                Console.WriteLine("4. Delete Artwork");
                Console.WriteLine("5. Delete Artist");
                Console.WriteLine("6. Count");
                Console.WriteLine("7. Search Artwork");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    choice = -1;
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Artist Name: ");
                        string aName = Console.ReadLine();
                        Console.Write("Country: ");
                        string country = Console.ReadLine();
                        Console.Write("Birth Year: ");
                        if (!int.TryParse(Console.ReadLine(), out int year))
                        {
                            Console.WriteLine(" Invalid year.");
                            break;
                        }
                        InsertArtist(aName, country, year);
                        break;

                    case 2:
                        Console.Write("Artist Id: ");
                        if (!int.TryParse(Console.ReadLine(), out int aid))
                        {
                            Console.WriteLine("Invalid artist id.");
                            break;
                        }
                        Console.Write("Title: ");
                        string title = Console.ReadLine();
                        Console.Write("Price: ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                        {
                            Console.WriteLine("Invalid price.");
                            break;
                        }
                        Console.Write("Type (Painting/Sculpture): ");
                        string type = Console.ReadLine();
                        Console.Write("Status (Available/Sold): ");
                        string status = Console.ReadLine();
                        InsertArtwork(aid, title, price, type, status);
                        break;

                    case 3:
                        SelectArtworks();
                        break;

                    case 4:
                        Console.Write("Enter Artwork Id to Delete: ");
                        int artId = Convert.ToInt32(Console.ReadLine());
                        DeleteArtwork(artId);
                        break;

                    case 5:
                        Console.Write("Enter Artist Id to Delete: ");
                        int artistId = Convert.ToInt32(Console.ReadLine());
                        DeleteArtwork(artistId);
                        break;

                    case 6:
                        CountTotal();
                        break;

                    case 7:
                        Console.Write("Search by Title/Type/Status: ");
                        string search = Console.ReadLine();
                        SearchArtwork(search);
                        break;

                    case 0:
                        Console.WriteLine("Exiting application...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (choice != 0)
                {
                    Console.WriteLine("\nPress Enter to continue...");
                    Console.ReadLine();
                }

            } while (choice != 0);
        }

        static void InsertArtist(string name, string country, int birthYear)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("InsertArtist", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Country", country);
                cmd.Parameters.AddWithValue("@BirthYear", birthYear);

                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Artist inserted successfully.");
            }
        }


        static void InsertArtwork(int artistId, string title, decimal price, string type, string status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("InsertArtwork", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ArtistId", artistId);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Status", status);

                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Artwork inserted successfully.");
            }
        }

        static void SelectArtworks()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetArtworksWithArtist", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("\nArtworks:");
                Console.WriteLine("ID | Title | Price | Type | Status | Artist");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["art_id"]} | {reader["title"]} | {reader["price"]} | {reader["art_type"]} | {reader["status"]} | {reader["artist_name"]}");

                }
            }
        }

        static void DeleteArtwork(int artId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteArtwork", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ArtId", artId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Artwork deleted successfully");
                }
                else
                {
                    Console.WriteLine("No artwork available to delete");
                }

            }
        }


        static void DeleteArtist(int artistId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteArtist", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ArtistId", artistId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    Console.WriteLine("Artist deleted successfully.");
                else
                    Console.WriteLine("No artist found with the given ID.");
            }
        }

        static void CountTotal()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("CountArtworksAndArtists", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter artistParam = new SqlParameter("@TotalArtists", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                SqlParameter artworkParam = new SqlParameter("@TotalArtworks", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(artistParam);
                cmd.Parameters.Add(artworkParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                Console.WriteLine($"Total Artists: {cmd.Parameters["@TotalArtists"].Value}");
                Console.WriteLine($"Total Artworks: {cmd.Parameters["@TotalArtworks"].Value}");
            }
        }


        static void SearchArtwork(string keyword)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SearchArtwork", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Search", keyword);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\nSearch Results:");
                Console.WriteLine("ID | Title | Price | Type | Status | Artist");

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["art_id"]} | {reader["title"]} | {reader["price"]} | {reader["art_type"]} | {reader["status"]} | {reader["artist_name"]}");
                }
            }
        }

    }
}
