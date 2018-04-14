#include "voice_chat.h"
using namespace vc_1;

#include <QDebug>

VoiceChat::VoiceChat(QObject *parent)
  : IVoiceChat(parent)
{

}

VoiceChat::~VoiceChat()
{

}


bool VoiceChat::initilize()
{

  return false;
}

void VoiceChat::start()
{

  qDebug() << "Stated";
}

void VoiceChat::free()
{

  qDebug() << "Free";
}
