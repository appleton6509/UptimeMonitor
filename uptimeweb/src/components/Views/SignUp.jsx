
import React, { Component } from 'react';
import { Card, Button, Container, Row, Col, CardBody, Form, FormGroup, Label, Input } from 'reactstrap';


export class SignUp extends Component {
    static displayName = SignUp.name;
    constructor(props) {
        super(props);
        this.state = {
            Username: "",
            Password: ""
        }
    }
    onSubmit = async (event) => {
        event.preventDefault();
        const body = {
            Username: this.state.Username,
            Password: this.state.Password
        }
        const jsonbody = JSON.stringify(body);
        const uri = 'https://localhost:44373/api/Auth/SignUp'
        const response = await fetch(uri, {
            method: 'POST',
            body: jsonbody,
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        }
        ).then(res => res.json())
            .catch(err => console.log("error:" + err));

        console.log("response:" + response);
    }
    handleUserChange = (e) => {
        this.setState({ Username: e.target.value })
    }
    handlePasswordChange = (e) => {
        this.setState({ Password: e.target.value })
    }

    render() {
        return (
            <Container>
                <Row className="justify-content-center" >
                    <Col md="6" >
                        <Card className="shadow mt-4">
                            <CardBody>
                                <Form onSubmit={this.onSubmit}>
                                    <FormGroup>
                                        <Label>Email / UserName</Label>
                                        <Input type="email" id="username" name="username" placeholder="email address" onChange={this.handleUserChange} />
                                    </FormGroup>
                                    <FormGroup>
                                        <Label>Password</Label>
                                        <Input type="password" id="password" name="password" placeholder="strong password goes here" onChange={this.handlePasswordChange} />
                                    </FormGroup>
                                    <FormGroup className="text-center">
                                        <Button type="submit" >OK</Button>
                                    </FormGroup>
                                </Form>
                            </CardBody>
                        </Card>
                    </Col>
                </Row>
            </Container>
        )
    }

}