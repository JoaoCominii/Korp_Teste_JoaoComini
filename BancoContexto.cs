using Microsoft.EntityFrameworkCore;

namespace Korp_Teste_JoaoComini.Data
{
    public class BancoContexto : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }

        public DbSet<NotaFiscal> NotasFiscais { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Cria o banco de dados em arquivo KorpTeste.db na pasta do projeto
            optionsBuilder.UseSqlite("Data Source=KorpTeste.db");
        }

protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar Produto com Codigo como chave primária
            modelBuilder.Entity<Produto>().HasKey(p => p.Codigo);

            // Configurar NotaFiscal com Numero como chave primária
            modelBuilder.Entity<NotaFiscal>().HasKey(n => n.Numero);

            // Converter List<tuple> para JSON para evitar aviso de entidade acidental
            modelBuilder.Entity<NotaFiscal>()
                .Property(n => n.Produtos)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<(string produto, int quantidade)>>(v)!);
        }
    }
    public class Produto
    {
        public string Codigo { get; set; } = "";

        public string Descricao { get; set; } = "";

        public int Saldo { get; set; }
    }

    public class NotaFiscal
    {
        public string Numero { get; set; } = "";

        public string Status { get; set; } = "Aberta";

        public List<(string produto, int quantidade)> Produtos { get; set; } = new();

        public int SaldoTotal { get; set; }
    }
}