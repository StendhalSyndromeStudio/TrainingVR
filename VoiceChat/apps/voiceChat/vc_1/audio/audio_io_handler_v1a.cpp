#include "audio_io_handler_v1a.h"
using namespace vc_1;

AudioIoHandler_v1a::AudioIoHandler_v1a(QObject *parent)
  : voice_chat::AudioIoHandler(parent)
{

}

AudioIoHandler_v1a::~AudioIoHandler_v1a()
{

}


bool vc_1::AudioIoHandler_v1a::isPlaying() const
{

  throw ( "Not implemented" ); //TODO
}

bool vc_1::AudioIoHandler_v1a::isRecording() const
{
  throw ( "Not implemented" ); //TODO
}


double vc_1::AudioIoHandler_v1a::recLevel() const
{
  throw ( "Not implemented" ); //TODO
}

double vc_1::AudioIoHandler_v1a::playLevel() const
{
  throw ( "Not implemented" ); //TODO
}

bool vc_1::AudioIoHandler_v1a::initilize()
{
  throw ( "Not implemented" ); //TODO
}

void vc_1::AudioIoHandler_v1a::free()
{
  throw ( "Not implemented" ); //TODO
}

voice_chat::AudioIoHandler::Error vc_1::AudioIoHandler_v1a::startPlay() const
{
  throw ( "Not implemented" ); //TODO
}

voice_chat::AudioIoHandler::Error vc_1::AudioIoHandler_v1a::startRecord() const
{
  throw ( "Not implemented" ); //TODO
}

void vc_1::AudioIoHandler_v1a::stopPlaying() const
{
  throw ( "Not implemented" ); //TODO
}

void vc_1::AudioIoHandler_v1a::stopRecording() const
{
  throw ( "Not implemented" ); //TODO
}
