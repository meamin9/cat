// Generated by github.com/davyxu/cellnet/protoc-gen-msg
// DO NOT EDIT!
// Source: chat.proto

package chatproto

import (
	"github.com/davyxu/cellnet"
	"reflect"
	_ "github.com/davyxu/cellnet/codec/pb"
)

func init() {

	// chat.proto
	cellnet.RegisterMessageMeta("pb", "chatproto.ReqChatText", reflect.TypeOf((*ReqChatText)(nil)).Elem(), 898475536)
	cellnet.RegisterMessageMeta("pb", "chatproto.ReqChatHistory", reflect.TypeOf((*ReqChatHistory)(nil)).Elem(), 2220442861)
	cellnet.RegisterMessageMeta("pb", "chatproto.S2C_ChatText", reflect.TypeOf((*S2C_ChatText)(nil)).Elem(), 3886291478)
	cellnet.RegisterMessageMeta("pb", "chatproto.S2C_ChatHistory", reflect.TypeOf((*S2C_ChatHistory)(nil)).Elem(), 340702852)

}
