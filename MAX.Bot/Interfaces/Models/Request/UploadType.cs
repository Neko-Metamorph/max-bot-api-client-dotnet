namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Тип загружаемого файла
/// </summary>
public enum UploadType
{
    /// <summary>
    /// Тип не задан
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Изображение: JPG, JPEG, PNG, GIF, TIFF, BMP, HEIC
    /// </summary>
    Image,

    /// <summary>
    /// Видео: MP4, MOV, MKV, WEBM, MATROSKA
    /// </summary>
    Video,

    /// <summary>
    /// Аудио: MP3, WAV, M4A и другие
    /// </summary>
    Audio,

    /// <summary>
    /// Любой файл
    /// </summary>
    File
}
