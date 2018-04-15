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

#include "vc_1/device/audio_device.h"
#include "vc_1/audio/audio_io_stream_v1a.h"

bool VoiceChat::initilize()
{
  auto stream = std::make_shared<AudioIoStream_v1a>();
  auto *device = new AudioDevice();

  device->play( stream );
  device->record( stream );


  return true;
}

void VoiceChat::start()
{

  qDebug() << "Stated";
}

void VoiceChat::free()
{

  qDebug() << "Free";
}
