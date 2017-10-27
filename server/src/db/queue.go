package db

type RetCode int

const (
	CodeSuccess RetCode = iota
	CodeFailed
	CodeAlreadyExist
	CodeNotFound
	CodeError
	CodeExit
)

type ISender interface {
	Send(*Request)
	Start()
	Stop()
	Poll()
}

type Request struct {
	query  func() (retdata interface{}, retcode RetCode)
	result func(interface{}, RetCode)
}

type Response func()

type queue struct {
	requests   chan *Request
	responses  chan *Response
	exitSignal chan bool
}

func (q *queue) Send(req *Request) {
	select {
	case q.requests <- req:
	default:
		log.Errorln("数据库发送队列已满")
		// 直接go一个，避免阻塞主线程
		go func() {
			q.requests <- req
		}()
	}
}

func (q *queue) waitRequest() {
	for {
		req := <-q.requests
		retdata, retcode := req.query()
		if retcode == CodeError {
			log.Errorln("Db Request error")
		} else if retcode == CodeExit {
			q.exitSignal <- true
			break
		}
		if req.result != nil {
			var response Response = func() {
				req.result(retdata, retcode)
			}
			q.responses <- &response
		}
	}
}

func (q *queue) Start() {
	go func() {
		q.waitRequest()
	}()
}

func (q *queue) Stop() {
	q.Send(&Request{
		func() (interface{}, RetCode) {
			return nil, CodeExit
		},
		nil,
	})
	<-q.exitSignal
}

func (q *queue) Poll() {
Loop:
	for {
		select {
		case res := <-q.responses:
			(*res)()
		default:
			break Loop
		}
	}
}

func NewQueue() *queue {
	return &queue{
		make(chan *Request, 100),
		make(chan *Response, 100),
		make(chan bool, 0),
	}
}
