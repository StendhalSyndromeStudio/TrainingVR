#ifndef AUDIO_IO_STREAM_H
#define AUDIO_IO_STREAM_H

#include <QObject>
#include <QByteArray>

namespace voice_chat {

  class AudioIoStream : public QObject
  {
    Q_OBJECT
  public:
    explicit AudioIoStream(QObject *parent = nullptr);
  public:
    virtual ~AudioIoStream();
  public:
    ///
    /// \brief Размер сохраненых данных
    ///
    virtual quint64 size() const = 0;
    ///
    /// \brief Размер буфера для данных
    ///
    virtual quint64 bufferSize() const = 0;

    ///
    /// \brief Получить данные из буфера
    /// \param size размер данных
    ///
    virtual QByteArray read(quint64 size) const = 0;

    ///
    /// \brief Получить данные из буфера
    /// \param data данные
    /// \param length длина
    ///
    virtual quint64 read(char *data, quint64 length) const = 0;

    template<typename AudioType>
    static quint64 read(AudioIoStream *stream, AudioType *data, quint64 length)
    {
      quint64 readLength = length * static_cast<quint64>( sizeof(AudioType) );
      return stream->read(reinterpret_cast<char *>(data), readLength );
    }
  public slots:
    ///
    /// \brief Записать данные
    /// \param data данные
    /// \return количество записанных данных
    ///
    virtual quint64 write(const QByteArray &data) = 0;
    ///
    /// \brief Записать данные
    /// \param data данные
    /// \param length количество данных
    /// \return количество записанных данных
    ///
    virtual quint64 write(const char *data, quint64 length) = 0;

    ///
    /// \brief take
    /// \param length
    ///
    virtual QByteArray take(quint64 length) = 0;
    ///
    /// \brief Вырезать данные из буфера
    /// \param data хранилище данных
    /// \param length длина данных
    /// \return количество прочитанных данных
    ///
    virtual quint64 take(char *data, quint64 length) = 0;

    virtual void clear();
  public:
    template<typename AudioType>
    static quint64 write(AudioIoStream *stream, AudioType *data, quint64 length)
    {
      quint64 readLength = length * static_cast<quint64>( sizeof(AudioType) );
      return stream->write(reinterpret_cast<char *>(data), readLength );
    }

    template<typename AudioType>
    static quint64 take(AudioIoStream *stream, AudioType *data, quint64 length)
    {
      quint64 readLength = length * static_cast<quint64>( sizeof(AudioType) );
      return stream->take(reinterpret_cast<char *>(data), readLength );
    }

  };

}



#endif // AUDIO_IO_STREAM_H
