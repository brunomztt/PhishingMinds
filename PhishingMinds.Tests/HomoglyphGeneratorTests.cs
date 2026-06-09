using EmailDispatcher.Application.Services.Utils;
using Xunit;

namespace PhishingMinds.Tests
{
    public class HomoglyphGeneratorTests
    {
        [Fact]
        public void Transform_NaoDeveRetornarNull()
        {
            var resultado =
                HomoglyphGenerator.Transform(
                    "Microsoft"
                );

            Assert.NotNull(resultado);
        }

        [Fact]
        public void Transform_DeveManterTamanho()
        {
            string texto = "Microsoft";

            var resultado =
                HomoglyphGenerator.Transform(texto);

            Assert.Equal(
                texto.Length,
                resultado.Length
            );
        }

        [Fact]
        public void Transform_TextoSemCaracteresSubstituiveis()
        {
            string texto = "VW";

            var resultado =
                HomoglyphGenerator.Transform(texto);

            Assert.Equal(
                texto,
                resultado
            );
        }

        [Fact]
        public void Transform_DeveRetornarStringValida()
        {
            var resultado =
                HomoglyphGenerator.Transform(
                    "Office 365"
                );

            Assert.False(
                string.IsNullOrWhiteSpace(
                    resultado
                )
            );
        }
    }
}