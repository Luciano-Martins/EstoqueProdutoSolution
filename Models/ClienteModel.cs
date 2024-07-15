using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstoqueWeb.Models
{
    [Table("Cliente")]
    public class ClienteModel : UsuarioModel
    {
        [Required , Column(TypeName = "char(14)")]
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        // criando uma  propriedade para retornar a idade do usuário tendo como basa data atual
        [NotMapped] // este atributo não deixa o framework mapear esta propriedade, evitando 
        //a criação de uma coluna  especifica dela.assim eu evito de maneira explicita
        public int Idade
        {
            get => (int)Math.Floor((DateTime.Now - DataNascimento).TotalDays / 365.2425);
            // {
            //     int dias = DateTime.Now.Subtract(DataNascimento).Days; 
            //     int anos = (int)Math.Floor(dias / 365.2425);// 2425 para resolver problemas de ano bissexto
            //     return anos ;

            // }
        }  
        public ICollection<EnderecoModel> Enderecos{ get; set; }
        public ICollection<PedidoModel>Pedidos {get; set;}

    }
}