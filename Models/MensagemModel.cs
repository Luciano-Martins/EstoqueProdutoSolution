using Newtonsoft.Json;

namespace EstoqueWeb.Models
{
    //
    public enum TipoMensagem
    {
        //criando um enumerado para representar 2 tipos de mensagem, informação (mensagem normal)
        // e Erro(Exibir mensagens de erro)
        Informacao , Erro
    }


    public class MensagemModel//criando a classe mensagemModel
    {
        public TipoMensagem Tipo { get; set; } // uma propriedade que é o Tipo da mensagem
        public string Texto { get; set; }//outra string que é o texto da mensagem

        //aqui eu crio um construtor da classe passando dois parametros , uma string (mensagem),
        //o outro uma TipoMensagem (tipo), já inicializando com TipoMensagem.Informação, se não
        //passar nada, esse sera o valor padrão
        public MensagemModel(string mensagem , TipoMensagem tipo = TipoMensagem.Informacao)
        {
           this.Tipo = tipo ; //indicando que Tipo recebe tipo
           this.Texto = mensagem;//Texto recebe a mensagem
           //por padrão esses metodos não podem serem transformados em textos
           //faremos abaixo dois métodos para fazerem isso

        }
       
       //o método Serealizar recebe uma string e um Tipo mensagem
        public static string Serializar(string mensagem , TipoMensagem tipo = TipoMensagem.Informacao)
        {
            //pegamos os dois parâmetros criamos nova MensagemModel e adicionamos os dois 
            //retornaremos o novo objeto convertido para string com a classe JsonConvert da biblioteca, 
            //Newtonsoft.Json
            var mensagemModel = new MensagemModel(mensagem ,tipo);
            return JsonConvert.SerializeObject(mensagemModel);
        }

         // esse método Desserializar recebe a string convert e retorna uma model
        public static MensagemModel Desserializar(string mensagemString)
        {
        // veja o retorno , utilizo JsonConvert com DeserializeObject passo a MensagemModel
        //para indicar que tipo quero que convert e no parametro passo a string que será convertida
            return JsonConvert.DeserializeObject<MensagemModel>(mensagemString);
        }
    }
}