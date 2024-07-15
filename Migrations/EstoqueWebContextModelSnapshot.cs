﻿// <auto-generated />
using System;
using EstoqueWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EstoqueWeb.Migrations
{
    [DbContext(typeof(EstoqueWebContext))]
    partial class EstoqueWebContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EstoqueWeb.Models.CategoriaModel", b =>
                {
                    b.Property<int>("IdCategoria")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("IdCategoria");

                    b.ToTable("categoria");
                });

            modelBuilder.Entity("EstoqueWeb.Models.ItemPedidoModel", b =>
                {
                    b.Property<int>("IdPedido")
                        .HasColumnType("int");

                    b.Property<int>("IdProduto")
                        .HasColumnType("int");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.Property<double>("ValorUnitario")
                        .HasColumnType("float");

                    b.HasKey("IdPedido", "IdProduto");

                    b.HasIndex("IdProduto");

                    b.ToTable("ItemPedido");
                });

            modelBuilder.Entity("EstoqueWeb.Models.PedidoModel", b =>
                {
                    b.Property<int>("IdPedido")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DataEntrega")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataPedido")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdCliente")
                        .HasColumnType("int");

                    b.Property<double?>("ValorTotal")
                        .HasColumnType("float");

                    b.HasKey("IdPedido");

                    b.HasIndex("IdCliente");

                    b.ToTable("Pedido");
                });

            modelBuilder.Entity("EstoqueWeb.Models.ProdutoModel", b =>
                {
                    b.Property<int>("IdProduto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Estoque")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int>("IdCategoria")
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<double>("Preco")
                        .HasColumnType("float");

                    b.HasKey("IdProduto");

                    b.HasIndex("IdCategoria");

                    b.ToTable("produto");
                });

            modelBuilder.Entity("EstoqueWeb.Models.UsuarioModel", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DataCadastro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Senha")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("IdUsuario");

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("EstoqueWeb.Models.ClienteModel", b =>
                {
                    b.HasBaseType("EstoqueWeb.Models.UsuarioModel");

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasColumnType("char(14)");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("datetime2");

                    b.ToTable("Cliente");
                });

            modelBuilder.Entity("EstoqueWeb.Models.ItemPedidoModel", b =>
                {
                    b.HasOne("EstoqueWeb.Models.PedidoModel", "Pedido")
                        .WithMany("ItensPedido")
                        .HasForeignKey("IdPedido")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EstoqueWeb.Models.ProdutoModel", "Produto")
                        .WithMany()
                        .HasForeignKey("IdProduto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pedido");

                    b.Navigation("Produto");
                });

            modelBuilder.Entity("EstoqueWeb.Models.PedidoModel", b =>
                {
                    b.HasOne("EstoqueWeb.Models.ClienteModel", "Cliente")
                        .WithMany("Pedidos")
                        .HasForeignKey("IdCliente")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("EstoqueWeb.Models.EnderecoModel", "EnderecoEntrega", b1 =>
                        {
                            b1.Property<int>("PedidoModelIdPedido")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Bairro")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("CEP")
                                .IsRequired()
                                .HasColumnType("char(9)");

                            b1.Property<string>("Cidade")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Complemento")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Estado")
                                .IsRequired()
                                .HasColumnType("char(2)");

                            b1.Property<string>("Logradouro")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Numero")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Referencia")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("PedidoModelIdPedido");

                            b1.ToTable("Pedido");

                            b1.WithOwner()
                                .HasForeignKey("PedidoModelIdPedido");
                        });

                    b.Navigation("Cliente");

                    b.Navigation("EnderecoEntrega");
                });

            modelBuilder.Entity("EstoqueWeb.Models.ProdutoModel", b =>
                {
                    b.HasOne("EstoqueWeb.Models.CategoriaModel", "Categoria")
                        .WithMany("Produtos")
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");
                });

            modelBuilder.Entity("EstoqueWeb.Models.ClienteModel", b =>
                {
                    b.HasOne("EstoqueWeb.Models.UsuarioModel", null)
                        .WithOne()
                        .HasForeignKey("EstoqueWeb.Models.ClienteModel", "IdUsuario")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.OwnsMany("EstoqueWeb.Models.EnderecoModel", "Enderecos", b1 =>
                        {
                            b1.Property<int>("IdUsuario")
                                .HasColumnType("int");

                            b1.Property<int>("IdEndereco")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Bairro")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("CEP")
                                .IsRequired()
                                .HasColumnType("char(9)");

                            b1.Property<string>("Cidade")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Complemento")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Estado")
                                .IsRequired()
                                .HasColumnType("char(2)");

                            b1.Property<string>("Logradouro")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Numero")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Referencia")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<bool>("Selecionado")
                                .HasColumnType("bit");

                            b1.HasKey("IdUsuario", "IdEndereco");

                            b1.ToTable("Endereco");

                            b1.WithOwner()
                                .HasForeignKey("IdUsuario");
                        });

                    b.Navigation("Enderecos");
                });

            modelBuilder.Entity("EstoqueWeb.Models.CategoriaModel", b =>
                {
                    b.Navigation("Produtos");
                });

            modelBuilder.Entity("EstoqueWeb.Models.PedidoModel", b =>
                {
                    b.Navigation("ItensPedido");
                });

            modelBuilder.Entity("EstoqueWeb.Models.ClienteModel", b =>
                {
                    b.Navigation("Pedidos");
                });
#pragma warning restore 612, 618
        }
    }
}
