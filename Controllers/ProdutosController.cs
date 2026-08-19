using Microsoft.AspNetCore.Mvc;
using Korp_Teste_JoaoComini.Data;
using Korp_Teste_JoaoComini.Services;

namespace Korp_Teste_JoaoComini.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly EstoqueService _estoqueService;

        public ProdutosController(EstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        // GET: api/produtos
        [HttpGet]
        public IActionResult ListarProdutos()
        {
            try
            {
                var codigos = _estoqueService.ObterCodigos();
                var produtos = new List<ProdutoDto>();

                foreach (var codigo in codigos)
                {
                    var info = _estoqueService.ObterProduto(codigo);
                    if (info.HasValue)
                    {
                        var (descricao, saldo) = info.Value;
                        produtos.Add(new ProdutoDto
                        {
                            Codigo = codigo,
                            Descricao = descricao,
                            Saldo = saldo
                        });
                    }
                }

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/produtos/{codigo}
        [HttpGet("{codigo}")]
        public IActionResult ObterProduto(string codigo)
        {
            try
            {
                var info = _estoqueService.ObterProduto(codigo);
                if (!info.HasValue)
                    return NotFound(new { error = $"Produto com código '{codigo}' não encontrado" });

                var (descricao, saldo) = info.Value;
                return Ok(new ProdutoDto
                {
                    Codigo = codigo,
                    Descricao = descricao,
                    Saldo = saldo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/produtos
        [HttpPost]
        public IActionResult CadastrarProduto([FromBody] ProdutoDto produto)
        {
            try
            {
                if (string.IsNullOrEmpty(produto.Codigo) || string.IsNullOrEmpty(produto.Descricao))
                    return BadRequest(new { error = "Código e Descrição são obrigatórios" });

                _estoqueService.CadastrarProduto(produto.Codigo, produto.Descricao, produto.Saldo);
                return CreatedAtAction(nameof(ObterProduto), new { codigo = produto.Codigo }, produto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT: api/produtos/{codigo}
        [HttpPut("{codigo}")]
        public IActionResult AtualizarProduto(string codigo, [FromBody] ProdutoDto produto)
        {
            try
            {
                _estoqueService.AtualizarProduto(codigo, produto.Descricao, produto.Saldo);
                return Ok(new { message = "Produto atualizado com sucesso" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE: api/produtos/{codigo}
        [HttpDelete("{codigo}")]
        public IActionResult RemoverProduto(string codigo)
        {
            try
            {
                _estoqueService.RemoverProduto(codigo);
                return Ok(new { message = "Produto removido com sucesso" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class ProdutoDto
    {
        public string Codigo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public int Saldo { get; set; }
    }
}
