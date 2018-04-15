#include "audio_stream_merger.h"
using namespace voice_chat;

AudioStreamMerger::AudioStreamMerger()
{

}
AudioStreamMerger::~AudioStreamMerger()
{

}


quint64 voice_chat::AudioStreamMerger::write(const QByteArray &data)
{
  Q_UNUSED(data);
  return 0;
}

quint64 voice_chat::AudioStreamMerger::write(const char *data, quint64 length)
{
  Q_UNUSED(data);
  return length;
}
