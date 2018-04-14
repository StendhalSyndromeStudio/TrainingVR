#ifndef IVOICE_CHAT_H
#define IVOICE_CHAT_H

#include <QObject>

namespace vc_1 {

  class IVoiceChat : public QObject
  {
    Q_OBJECT
  protected:
    explicit IVoiceChat(QObject *parent = nullptr);
  public:
    virtual ~IVoiceChat();
  signals:
    void closeApplication();
  public slots:
    virtual bool initilize() = 0;
    virtual void start() = 0;
    virtual void free() =  0;
  };

}



#endif // IVOICE_CHAT_H
