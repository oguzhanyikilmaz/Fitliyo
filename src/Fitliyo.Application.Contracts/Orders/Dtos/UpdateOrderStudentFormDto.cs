using System.ComponentModel.DataAnnotations;
using Fitliyo.Orders;

namespace Fitliyo.Orders.Dtos;

/// <summary>
/// Öğrencinin sipariş için eğitmene ilettiği bilgiler (kan değerleri, hedefler, notlar vb.).
/// JSON string veya serbest metin olarak gönderilebilir.
/// </summary>
public class UpdateOrderStudentFormDto
{
    /// <summary>
    /// Form verisi (JSON veya düz metin). Örn. kan değerleri, kronik rahatsızlıklar, hedef, diyet kısıtları.
    /// </summary>
    [StringLength(OrderConsts.MaxStudentFormDataLength)]
    public string? FormData { get; set; }
}
