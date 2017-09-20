OUTDIR="proto_go"
mkdir $OUTDIR
protoc --plugin=protoc-gen-go --go_out $OUTDIR --proto_path "." chat.proto
protoc --plugin=protoc-gen-msg --msg_out=msgid.go:$OUTDIR --proto_path "." chat.proto
