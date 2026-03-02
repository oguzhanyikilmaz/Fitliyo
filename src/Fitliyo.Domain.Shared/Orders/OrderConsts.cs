namespace Fitliyo.Orders;

public static class OrderConsts
{
    public const int MaxOrderNumberLength = 20;
    public const int MaxCurrencyLength = 3;
    public const int MaxCancellationReasonLength = 500;
    public const int MaxNotesLength = 1000;
    public const int MaxPaymentTransactionIdLength = 128;
    public const int MaxPaymentProviderLength = 50;
    public const int MaxMeetingUrlLength = 512;
    public const int MaxSessionNotesLength = 1000;
    public const int MaxLocationLength = 256;
    /// <summary>Öğrencinin sipariş için gönderdiği form verisi (JSON, kan değerleri vb.)</summary>
    public const int MaxStudentFormDataLength = 8000;
    /// <summary>Eğitmenin program teslim notu</summary>
    public const int MaxTrainerProgramNotesLength = 4000;
    /// <summary>Program dosyası/link (PDF vb.)</summary>
    public const int MaxProgramAttachmentUrlLength = 512;
    public const string DefaultCurrency = "TRY";
    public const decimal PlatformCommissionRate = 0.15m;
}
