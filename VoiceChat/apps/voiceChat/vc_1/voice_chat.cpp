#include "voice_chat.h"
using namespace vc_1;

#include <QIcon>
#include <QMenu>
#include <QDebug>

VoiceChat::VoiceChat(QObject *parent)
  : IVoiceChat(parent)
{

  _tray.setIcon( QIcon( ":/icon/tray.png" ) );

  _tray.setContextMenu( new QMenu() );
  auto *a = _tray.contextMenu()->addAction( "Выход" );

  connect( a,     &QAction::triggered,
           this,  &VoiceChat::closeApplication );

  _tray.show();
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

  _tray.showMessage( "Информация", "Модуль голосовой связи запущен" );
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
