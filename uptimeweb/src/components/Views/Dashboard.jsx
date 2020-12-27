import React, { Component } from 'react';
import { CardTitle, Card, Container, Row, Col, CardBody } from 'reactstrap';
import fetchservice from '../Services/fetchservice';
import {API_URI} from "../Settings/globals";

export class Dashboard extends Component {
    static displayName = Dashboard.name;

    getEchoes = async () => {
        var req = new fetchservice();
        var uri = API_URI + "Echoes/Get";
        let data = await req.get(uri).then(res=> {
            if (res.ok)
                return res.text();
        });
        console.log(data);


    }
    render() {
        return (
            <Container>
                <Row >
                    <Col lg="4">
                        <div className="shadow mt-4">
                            <Card>
                                <CardTitle className="text-center">Online</CardTitle>
                                <CardBody>

                                </CardBody>
                            </Card>
                        </div>
                    </Col>
                </Row>
            </Container>
        );
    }
}