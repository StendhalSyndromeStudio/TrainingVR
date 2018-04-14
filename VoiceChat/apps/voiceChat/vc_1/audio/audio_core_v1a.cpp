#include "audio_core_v1a.h"
using namespace vc_1;


AudioCore_v1a::AudioCore_v1a(QObject *parent) : QObject(parent)
{

}

AudioCore_v1a::~AudioCore_v1a()
{

}


voice_chat::AudioIoHandler vc_1::AudioCore_v1a::audioHandler() const
{
  throw ( "Not implemented" ); //TODO
}

voice_chat::Mode vc_1::AudioCore_v1a::mode() const
{
  throw ( "Not implemented" ); //TODO
}

bool vc_1::AudioCore_v1a::initilize(Mode mode)
{
  Q_UNUSED(mode);
  throw ( "Not implemented" ); //TODO
}

void vc_1::AudioCore_v1a::free()
{
  throw ( "Not implemented" ); //TODO
}
