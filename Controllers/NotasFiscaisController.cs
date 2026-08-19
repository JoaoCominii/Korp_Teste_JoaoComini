using Microsoft.AspNetCore.Mvc;
using Korp_Teste_JoaoComini.Data;
using Korp_Teste_JoaoComini.Services;

namespace Korp_Teste_JoaoComini.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly FaturamentoService _faturamentoService;
        private readonly EstoqueService _estoqueService;

        public NotasFiscaisController(FaturamentoService faturamentoService, EstoqueService estoqueService)
        {
            _faturamentoService = faturamentoService;
            _estoqueService = estoqueService;
        }

        // GET: api/notasfiscais
        [HttpGet]
        public IActionResult ListarNotasFiscais()
        {
            try
            {
                var notas = _faturamentoService.ListarNotasFiscais();
                var notasDto = notas.Select(n => new NotaFiscalDto
                {
                    Numero = n.Numero,
                    Status = n.Status,
                    Produtos = n.Produtos.Select(p => new ProdutoNotaDto
                    {
                        Codigo = p.Codigo,
                        Quantidade = p.Quantidade
                    }).ToList(),
                    SaldoTotal = n.SaldoTotal
                }).ToList();

                return Ok(notasDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/notasfiscais/{numero}
        [HttpGet("{numero}")]
        public IActionResult ObterNotaFiscal(string numero)
        {
            try
            {
                var notas = _faturamentoService.ListarNotasFiscais();
                var nota = notas.FirstOrDefault(n => n.Numero == numero);

                if (nota == null)
                    return NotFound(new { error = $"Nota fiscal '{numero}' não encontrada" });

                return Ok(new NotaFiscalDto
                {
                    Numero = nota.Numero,
                    Status = nota.Status,
                    Produtos = nota.Produtos.Select(p => new ProdutoNotaDto
                    {
                        Codigo = p.Codigo,
                        Quantidade = p.Quantidade
                    }).ToList(),
                    SaldoTotal = nota.SaldoTotal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/notasfiscais
        [HttpPost]
        public IActionResult CadastrarNotaFiscal([FromBody] CriarNotaFiscalDto dto)
        {
            try
            {
                if (dto.Produtos == null || dto.Produtos.Count == 0)
                    return BadRequest(new { error = "Nota fiscal deve conter pelo menos um produto" });

                var produtos = dto.Produtos.Select(p => new ProdutoNota { Codigo = p.Codigo, Quantidade = p.Quantidade }).ToList();

                // Validate products exist
                foreach (var pn in produtos)
                {
                    if (!_estoqueService.ObterProduto(pn.Codigo).HasValue)
                        return BadRequest(new { error = $"Produto com código '{pn.Codigo}' não encontrado" });
                }

                _faturamentoService.CadastrarNotafiscal(produtos);

                return CreatedAtAction(nameof(ObterNotaFiscal), new { numero = "NF-" + DateTime.Now.Year }, new { message = "Nota fiscal criada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/notasfiscais/{numero}/imprimir
        [HttpPost("{numero}/imprimir")]
        public IActionResult ImprimirNotaFiscal(string numero)
        {
            try
            {
                var notas = _faturamentoService.ListarNotasFiscais();
                var nota = notas.FirstOrDefault(n => n.Numero == numero);

                if (nota == null)
                    return NotFound(new { error = $"Nota fiscal '{numero}' não encontrada" });

                if (nota.Status != "Aberta")
                    return BadRequest(new { error = "Apenas notas com status 'Aberta' podem ser impressas" });

                bool resultado = _faturamentoService.ImprimirNotafiscal(numero, _estoqueService);

                if (resultado)
                {
                    return Ok(new { message = "Nota fiscal impressa com sucesso", status = "Fechada" });
                }
                else
                {
                    return BadRequest(new { error = "Falha ao imprimir nota fiscal" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class NotaFiscalDto
    {
        public string Numero { get; set; } = "";
        public string Status { get; set; } = "";
        public List<ProdutoNotaDto> Produtos { get; set; } = new();
        public int SaldoTotal { get; set; }
    }

    public class ProdutoNotaDto
    {
        public string Codigo { get; set; } = "";
        public int Quantidade { get; set; }
    }

    public class CriarNotaFiscalDto
    {
        public List<ProdutoNotaDto> Produtos { get; set; } = new();
    }
}
