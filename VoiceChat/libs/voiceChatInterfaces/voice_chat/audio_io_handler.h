#ifndef ABSTRACT_AUDIO_IO_HANDLER_H
#define ABSTRACT_AUDIO_IO_HANDLER_H

#include <QObject>

namespace voice_chat {

  ///
  /// \brief Обработчик ввода вывода звуковых данных
  ///
  class AudioIoHandler : public QObject
  {
    Q_OBJECT
  public:
    ///
    /// \brief Ошибки обработчика
    ///
    enum class Error: quint64
    {
      NoError = 0,
      UnknowError
    };
    Q_ENUM(Error)
  protected:
    explicit AudioIoHandler(QObject *parent = nullptr);
  public:
    virtual ~AudioIoHandler();
  public:
    ////
    /// \brief Осуществляется ли воспроизведение в данный момент
    ///
    virtual bool isPlaying() const = 0;
    ///
    /// \brief Осуществляется ли в данный момент доступ к микрофону
    ///
    virtual bool isRecording() const = 0;

    ///
    /// \brief Текущий уровень микрофона
    ///
    virtual double recLevel() const = 0;

    ///
    /// \brief Текущий уровень воспроизведения
    ///
    virtual double playLevel() const = 0;

  signals:
    ///
    /// \brief Ошибка доступа к микрофону
    /// \param error код ошибки
    /// \param errorString текст ошибки
    ///
    void audioInputError(Error error, const QString &errorString);
    ///
    /// \brief Ошибка доступа к устройству воспроизведения
    /// \param error код ошибки
    /// \param errorString текст ошибки
    ///
    void audioOutputError(Error error, const QString &errorString);

    ///
    /// \brief Обновлено состояние воспроизведения
    ///
    void plauingUpdateState(bool playing);
    ///
    /// \brief Обновлено состояние текущей записи
    /// \todo Изменить описание а то не корректно звучит
    ///
    void recordingUpdateState(bool recording);
  public slots:
    ///
    /// \brief Метод инициализации класса
    ///
    virtual bool initilize() = 0;
    ///
    /// \brief Метод освобождения ресурсов класса
    ///
    virtual void free() = 0;

    ///
    /// \brief Начать воспроизведение данных
    ///
    virtual Error startPlay() const = 0;

    ///
    /// \brief Остановитьвоспроизведение данных
    ///
    virtual Error startRecord() const = 0;

    ///
    /// \brief Остановить воспроизведение
    ///
    virtual void stopPlaying() const = 0;

    ///
    /// \brief Остановить запись данных с микрофона
    ///
    virtual void stopRecording() const = 0;
  };

}

#endif // ABSTRACT_AUDIO_IO_HANDLER_H
