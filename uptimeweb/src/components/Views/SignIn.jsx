import React, { Component } from 'react';
import { Card, Button, Container, Row, Col, CardBody, Form, 
    FormGroup, Label, Input,PopoverBody, UncontrolledPopover } from 'reactstrap';


export class SignIn extends Component {
    static displayName = SignIn.name;
    constructor(props) {
        super(props);
        this.state = {
            Username: "",
            Password: "",
            httperror: "",
            popoverOpen: false,
            token: "",
            tokenReceived: false
        }
    }

    setPopoverOpen = () => {
        this.setState({ popoverOpen: !this.popoverOpen })
    }
    postSignIn = async (jsonbody, uri) => {
        this.setState({ error: "" });
        await fetch(uri, {
            method: 'POST',
            body: jsonbody,
            headers: {
                'Accept': '*/*',
                'Content-Type': 'application/json'
            }
        }).then(res => {
            if (res.ok)
                this.setState({tokenReceived: true})
            return res.text()       
        }).then(message => {
            if (this.state.tokenReceived)
                this.setState({token: message});
            else 
                this.setState({ httperror: message })
        }).catch(err => {
            this.setState({ httperror: "something went wrong" })
        });
    }
    onSubmit = async (event) => {
        event.preventDefault();
        const body = {
            Username: this.state.Username,
            Password: this.state.Password
        }
        const uri = 'https://localhost:44373/api/Auth/SignIn';
        await this.postSignIn(JSON.stringify(body), uri);
        if (this.state.httperror) {
            console.log(this.state.httperror);
        }
        console.log("token:"+this.state.token);
        console.log("tokenreceived?:"+this.state.tokenReceived);
        //else 
        //do something
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
                <Row>
                    <Col>
                        <h1 className="mt-3 mb-3 text-center">
                        Lets get you LOGGED IN.
                        </h1> 
                    </Col>
                </Row>
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
                                        <Button type="submit" id="btnSubmit">OK</Button>
                                    </FormGroup>
                                </Form>
                                <UncontrolledPopover isOpen={this.state.popoverOpen} trigger="focus click" placement="bottom"
                                    toggle={this.setPopoverOpen} target="btnSubmit">
                                    <PopoverBody>{this.state.httperror}</PopoverBody>
                                </UncontrolledPopover>
                            </CardBody>
                        </Card>
                    </Col>
                </Row>

            </Container>
        )
    }

}