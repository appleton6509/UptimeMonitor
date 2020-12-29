import React, { Component } from 'react';
import { CardTitle, Card, Container, Row, Col, CardBody, Spinner } from 'reactstrap';
import {fetchapi} from '../Services/fetchservice';

export class ManageEndPoints extends Component {
   // static displayName = ManageEndPoints.name;
    constructor(props) {
        super(props);
        this.state = {
            isFetching: false,
            endpoints: []
        }
    }
    componentDidMount() {
        this.fetchEndPoints();
        this.timer = setInterval(()=> this.fetchEndPoints(), 60000);
    }
    componentWillUnmount() {
        clearInterval(this.timer);
        this.timer = null;
    }

    fetchEndPoints = async () => {
        this.setState({isFetching: true});
        let errorCode;
        const data = await fetchapi('EndPoints',"GET").then(res => {
            if (res.ok)
                return res.json();
            else 
                throw res.status
        }).then(data => {
            return data;
        }).catch(err => {
            errorCode = err;
        });

        this.setState({isFetching: false});
    }
    render() {
        let display = this.state.isFetching ? <Spinner/> : <div></div>;
        return (
            <Container>
                <Row>
                    <Col lg="12" >
                        <div className="shadow mt-4">
                            <Card>
                                <CardTitle className="text-center">Top 10 Endpoints with Errors</CardTitle>
                                <CardBody>
                               {display}
                                </CardBody>
                            </Card>
                        </div>
                    </Col>
                </Row>
            </Container>
        );
    }
}