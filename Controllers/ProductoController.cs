using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_Core.Models;

using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace API_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly string SQLString;

        public ProductoController(IConfiguration config)
        {
            SQLString = config.GetConnectionString("SQLString");
        }

        [HttpGet]
        [Route("ListProducts")]
        public IActionResult GetListProducts() 
        {
            List<Producto> listProducts = new List<Producto>();
            try
            {
                using (var conn = new SqlConnection(SQLString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_lista_productos", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listProducts.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                CodigoBarra = reader["CodigoBarra"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Marca = reader["Marca"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(reader["Precio"])
                            }); 
                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = listProducts });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, response = listProducts });
            }
        }

        [HttpGet]
        [Route("Product/{id:int}")]
        public IActionResult GetProduct(int id)
        {
            try
            {
                var product = new Producto();

                using (var conn = new SqlConnection(SQLString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_devuelve_producto", conn);
                    cmd.Parameters.AddWithValue("IdProducto", id);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Producto()
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                CodigoBarra = reader["CodigoBarra"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Marca = reader["Marca"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(reader["Precio"])
                            };
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status404NotFound, new { message = "Not Item Found", response = product });
                        }

                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = product });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, response = ex.InnerException.Message });
            }
        }
    }
}
