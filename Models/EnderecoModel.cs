using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Models
{
    [Owned,Table("Endereco")]//esse atributo indica que essa entidade pertence a outra entidade
    public class EnderecoModel
    {
      public int IdEndereco { get; set;}
      [Required]
      public string Logradouro { get; set; }
        [Required]
      public string Numero { get; set; }
       
      public string Complemento { get; set; }
      [Required]
      public string Bairro {get ; set;}
      [Required]
      public string Cidade {get ; set ;}
      [Required, Column(TypeName ="char(2)")]
      public string Estado { get; set; }
      [Required ,Column(TypeName = "char(9)")]
      public string CEP { get; set; }
      public string Referencia { get; set; }
      public bool Selecionado { get; set ;}
      [NotMapped]
      public string EnderecoCompleto 
      {
        get{
          return $"{Logradouro},{Numero},{Complemento},{Bairro},{Cidade},{Estado},CEP{CEP}";
        }
      }
    }
}