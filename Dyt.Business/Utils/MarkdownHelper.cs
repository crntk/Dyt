using Markdig;

namespace Dyt.Business.Utils
{
    /// <summary>
    /// Markdown içeriðini HTML'e dönüþtürmek için yardýmcý sýnýf
    /// </summary>
    public static class MarkdownHelper
    {
     private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions() // Tablolar, liste, vurgu vb. için geliþmiþ özellikler
            .Build();

        /// <summary>
        /// Markdown içeriðini güvenli HTML'e çevirir
        /// </summary>
        /// <param name="markdown">Markdown formatýndaki metin</param>
        /// <returns>HTML formatýnda metin</returns>
      public static string ToHtml(string? markdown)
    {
      if (string.IsNullOrWhiteSpace(markdown))
     return string.Empty;

            return Markdown.ToHtml(markdown, _pipeline);
        }
    }
}
