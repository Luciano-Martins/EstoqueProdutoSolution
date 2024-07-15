using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EstoqueWeb.Models
{
    //vamos criar a classe  EtoqueWebContext(BancoDados) que vai herdar DbContext(EntityFramework)
    //representa um contexto de BancoDados
    public class EstoqueWebContext : DbContext
    {
        // agora preciso informar quais coleções de objetos que esse contexto vai ter
        //DbSet representa uma coleção de Objetos, tipo CategoriaModel, vou chamar de categoria
        public DbSet<CategoriaModel> Categorias { get; set; }
        //agora vou fazer um construtor,para que o objeto de contexto de banco de dados
        //possa ser obtido atraveis de injeção de dependência

        // um construtor que permite a passagem de um conjunto de opções para o banco de dados

        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<ItemPedidoModel> ItensPedido { get; set; }
        public EstoqueWebContext(DbContextOptions<EstoqueWebContext> options) : base(options)
        {

        }
        //esse aqui é um codigo totalmente opcional, ele é adicionado para criar a table como o nome de 
        //categoria, caso não, a table terá o no de Categorias
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoriaModel>().ToTable("categoria");
            modelBuilder.Entity<ProdutoModel>().ToTable("produto");
            modelBuilder.Entity<ClienteModel>().OwnsMany(c => c.Enderecos, e =>
            {
                e.WithOwner().HasForeignKey("IdUsuario");
                e.HasKey("IdUsuario", "IdEndereco");
            });
            modelBuilder.Entity<UsuarioModel>().Property(u => u.DataCadastro)
           .HasDefaultValueSql("GETDATE()")
           .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<ProdutoModel>().Property(p => p.Estoque).HasDefaultValue(0);
            modelBuilder.Entity<PedidoModel>().OwnsOne(p => p.EnderecoEntrega, e =>
            {
                e.Ignore(e => e.IdEndereco);
                e.Ignore(e => e.Selecionado);
                e.ToTable("Pedido");
            });
            modelBuilder.Entity<ItemPedidoModel>().HasKey(ip => new { ip.IdPedido, ip.IdProduto });
        }
    }
}