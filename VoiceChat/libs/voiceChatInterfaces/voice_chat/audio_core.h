#ifndef AUDIO_CORE_H
#define AUDIO_CORE_H

#include <QObject>

namespace voice_chat {

  class AudioIoHandler;

  ///
  /// \brief Ядро управления вводом и выводом
  ///
  class AudioCore : public QObject
  {
    Q_OBJECT
  public:
    enum class Mode: quint8
    {
      Master, Slave
    };
    Q_ENUM(Mode)
  protected:
    explicit AudioCore(QObject *parent = nullptr);
  public:
    virtual ~AudioCore();
  public:
    ///
    /// \brief Обработчик аудио
    ///
    virtual AudioIoHandler *audioHandler() const = 0;

    ///
    /// \brief Текущий режим работы
    ///
    virtual Mode mode() const = 0;
  signals:

  public slots:
    ///
    /// \brief Инициализация
    /// \param mode режим работы
    ///
    virtual bool initilize(Mode mode) = 0;
    ///
    /// \brief Освободить ресурсы класса
    ///
    virtual void free() = 0;
  };

}

#endif // AUDIO_CORE_H
