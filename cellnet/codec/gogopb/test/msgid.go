// Generated by cellnet/protoc-gen-msg
// DO NOT EDIT!
// Source: pb.proto

package test

import (
	"cellnet"
	"cellnet/codec"
	_ "cellnet/codec/gogopb"
	"reflect"
)

func init() {

	// pb.proto
	cellnet.RegisterMessageMeta(&cellnet.MessageMeta{
		Codec: codec.MustGetCodec("gogopb"),
		Type:  reflect.TypeOf((*ContentACK)(nil)).Elem(),
		ID:    60952,
	})
}