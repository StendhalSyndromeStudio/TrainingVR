#ifndef AUDIO_CORE_H
#define AUDIO_CORE_H

#include <QObject>

namespace voice_chat {

  ///
  /// \brief Ядро управления вводом и выводом
  ///
  class AudioCore : public QObject
  {
    Q_OBJECT
  protected:
    explicit AudioCore(QObject *parent = nullptr);
  public:
    virtual ~AudioCore();
  public:


  signals:

  public slots:
    virtual bool initilize() = 0;
    virtual void free() = 0;
  };

}

#endif // AUDIO_CORE_H
