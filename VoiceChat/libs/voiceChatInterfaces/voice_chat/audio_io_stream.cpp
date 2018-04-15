#include "audio_io_stream.h"
using namespace voice_chat;

AudioIoStream::AudioIoStream(QObject *parent)
  : QObject(parent)
{

}

AudioIoStream::~AudioIoStream()
{

}

void AudioIoStream::clear()
{
  take( size() );
}
