import React, { PureComponent } from 'react';
import { Card, Button, Container, Row, Col, CardBody, Form, 
    FormGroup, Label, Input, Spinner} from 'reactstrap';
import { createLogin } from '../Services/authservice'
import {ToastContainer, toast} from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export class SignUp extends PureComponent {
    static displayName = SignUp.name;
    constructor(props) {
        super(props);
        this.state = {
            username: "",
            password: "",
            isLoading: false
        }
    }
    onSubmit = async (event) => {
        event.preventDefault();
        this.setState({isLoading: true});
        let result = await createLogin(event.target.username.value,event.target.password.value);
        if (result.success) {
            toast.success("Success");
            window.location.replace("/");
        }
        else
            toast.error(result.error);
            setTimeout(()=>{this.setState({isLoading: false});},500);
        
    }

    render() {
        return (
            <Container>
                   <ToastContainer
                    position="bottom-center"
                    autoClose={5000}
                    hideProgressBar
                    newestOnTop={false}
                    closeOnClick
                    rtl={false}
                    pauseOnFocusLoss
                    draggable
                    pauseOnHover
                    />
                <Row>
                    <Col>
                        <h1 className="mt-3 mb-3 text-center">
                        Create a FREE Account.
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
                                        <Input type="text" formNoValidate required={false} id="username" name="username" 
                                        placeholder="email address" />
                                    </FormGroup>
                                    <FormGroup>
                                        <Label>Password</Label>
                                        <Input type="password" formNoValidate required={false} id="password" name="password" 
                                         placeholder="strong password goes here" />
                                    </FormGroup>
                                    <FormGroup className="text-center">
                                    <Button  type="submit" id="btnSubmit" className="mb-4">
                                        <div hidden={this.state.isLoading}> OK </div>
                                        <Spinner hidden={!this.state.isLoading} as="span" animation="grow" size="sm" role="status" aria-hidden="true" />
                                        </Button>
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